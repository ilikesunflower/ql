using System;
using System.Threading;
using System.Threading.Tasks;
using CMS.Extensions.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CMS.Extensions.Notification
{
    public class NotificationQueuedHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly INotificationBackgroundTaskQueue _taskQueue;
        private readonly int _executorsCount = 1; //--default value: 2
        private readonly Task[] _executors;
        private CancellationTokenSource _tokenSource;


        public NotificationQueuedHostedService(INotificationBackgroundTaskQueue taskQueue, IServiceScopeFactory serviceScopeFactory,
            ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = loggerFactory.CreateLogger<NotificationQueuedHostedService>();
            _executorsCount = configuration.GetSection("WebSetting").GetValue<int>("NotificationQueueMaxCount");
            _executors = new Task[_executorsCount];
        }

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
                            catch (Exception)
                            {
                                // ignored
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
