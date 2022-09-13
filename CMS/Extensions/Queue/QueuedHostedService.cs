using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace CMS.Extensions.Queue
{
    public class QueuedHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly int _executorsCount = 1; //--default value: 2
        private readonly Task[] _executors;
        private CancellationTokenSource _tokenSource;


        public QueuedHostedService(IBackgroundTaskQueue taskQueue, IServiceScopeFactory serviceScopeFactory,
            ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
            _executorsCount = configuration.GetSection("WebSetting").GetValue<int>("ExecutorsQueueMaxCount");
            _executors = new Task[_executorsCount];
        }

        [SuppressMessage("ReSharper.DPA", "DPA0002: Excessive memory allocations in SOH", MessageId = "type: System.String")]
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            for (var i = 0; i < _executorsCount; i++)
            {
                async void Action()
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var workItem = await _taskQueue.DequeueAsync(cancellationToken);
                        if (workItem is not null)
                        {
                            try
                            {
                                await workItem(_serviceScopeFactory, cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                            }
                        }
                    }
                }
                var executorTask = new Task(Action, _tokenSource.Token);
                _executors[i] = executorTask;
                executorTask.Start();
            }
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            if (_executors != null)
            {
                Task.WaitAll(_executors, cancellationToken);
            }
            return Task.CompletedTask;
        }
    }
}