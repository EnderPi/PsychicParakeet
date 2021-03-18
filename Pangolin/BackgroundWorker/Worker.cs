using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.Messaging;

namespace BackgroundWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private BackgroundWorkerRuntime _runtime;

        private EventManager _eventManager;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        /// <summary>
        /// Starts the service.  Need to create a background worker runtime and start it.  Obviously, need a lot of services.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //load up configs

            //load up global configs

            //load up app configs




            _eventManager.StartListening();
            _runtime.Start();
            return base.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Stops the service.  Probably just call stop on the event manager and background workerruntime.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _eventManager.StopListening();
            _runtime.Stop();
            return base.StopAsync(cancellationToken);
        }

    }
}
