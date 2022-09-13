using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.Extensions.Queue
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<IServiceScopeFactory, CancellationToken, Task> workItem);

        Task<Func<IServiceScopeFactory, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        int GetCountWorkItem();
    }
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<IServiceScopeFactory, CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<IServiceScopeFactory, CancellationToken, Task>>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public BackgroundTaskQueue()
        {

        }

        public void QueueBackgroundWorkItem(Func<IServiceScopeFactory, CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                return;
            }
            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<IServiceScopeFactory, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);
            return workItem;
        }

        public int GetCountWorkItem()
        {
            return _workItems.Count;
        }
    }
}
