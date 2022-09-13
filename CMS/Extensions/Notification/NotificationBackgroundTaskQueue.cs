using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.Extensions.Notification
{
    public interface INotificationBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<IServiceScopeFactory, CancellationToken, Task> workItem);

        Task<Func<IServiceScopeFactory, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public class NotificationBackgroundTaskQueue : INotificationBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<IServiceScopeFactory, CancellationToken, Task>> _workItems = new ConcurrentQueue<Func<IServiceScopeFactory, CancellationToken, Task>>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public NotificationBackgroundTaskQueue()
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
    }
}
