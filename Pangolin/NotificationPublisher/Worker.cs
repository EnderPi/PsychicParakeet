using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EnderPi.Framework.Threading;
using EnderPi.Framework.Messaging;
using EnderPi.Framework.Caching;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Logging;
using System.Data;

namespace NotificationPublisher
{

    /// <summary>
    /// The service that routes messages to subscribing applications.  
    /// </summary>
    public class Worker : BackgroundService
    {
        
        /// <summary>
        /// The local event manager, since this is an application too.
        /// </summary>
        private EventManager _eventManager;
        
        /// <summary>
        /// This is a default thing with .net Core.  I dislike it.
        /// </summary>
        private readonly ILogger<Worker> _logger;

        /// <summary>
        /// Application settings, from the daterbase.
        /// </summary>
        private NotificationPublisherAppSettings _settings;

        /// <summary>
        /// The receiving, or global, event queue.
        /// </summary>
        private IMessageQueue _receivingQueue;

        /// <summary>
        /// The logger.
        /// </summary>
        private Logger _myLogger;

        /// <summary>
        /// Dater access for the logger.
        /// </summary>
        private LogDataAccess _logDataAccess;

        /// <summary>
        /// Configuration data access.
        /// </summary>
        private IConfigurationDataAccess _configurationDataAccess;
        
        /// <summary>
        /// Injected parameters from the app config.
        /// </summary>
        private readonly WorkerOptions _options;

        private NotificationPublisherRuntime _notificationPublisher;

        /// <summary>
        /// Constructor, injects a few services.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <param name="configurationDataAccess"></param>
        /// <param name="logDataAccess"></param>
        public Worker(ILogger<Worker> logger, WorkerOptions options, ConfigurationDataAccess configurationDataAccess, LogDataAccess logDataAccess)
        {
            _logger = logger;
            _options = options;
            _configurationDataAccess = configurationDataAccess;
            _logDataAccess = logDataAccess;
        }

        /// <summary>
        /// Probably don't need this chatter, this came with the service.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Notification Publisher Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }

        /// <summary>
        /// Starts the service.  Method feels a bit messy, need to extract stuff and consolidate.  Basically sets up all the state.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// There's quite a bit of state, since this has a throttledtaskprocessor and an event processor.  Ironically, most of the work is 
        /// setting up the state of everything.  The classes do most of the heavy lifting.
        /// </remarks>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //todo needs an initialized boolean so that it doesn't re-initialize.  That will cause a leak in the logger, and also maybe hang a monitored cache?
            _settings = _configurationDataAccess.GetApplicationConfigurationValues<NotificationPublisherAppSettings>(_options.ApplicationName);
            
            _myLogger = new Logger(_logDataAccess, _options.ApplicationName, _settings.ApplicationLoggingLevel);        //todo if we ever want to change log levels on the fly we'll need to hook a monitored cache into a config provider that gets injected into the logger

            var publishingEventQueue = _configurationDataAccess.GetGlobalSettingString(GlobalSettings.EventPublishingQueue);
            _receivingQueue = new MessageQueue(_options.ConnectionString, publishingEventQueue);

            var applicationEventQueue = new MessageQueue(_options.ConnectionString, _settings.EventQueueName);
            var taskParameters = new ThrottledTaskProcessorParameters(1, 30, 4000, 120, false);
            _eventManager = new EventManager(_options.ConnectionString, _receivingQueue, applicationEventQueue, taskParameters, _myLogger);


            _notificationPublisher = new NotificationPublisherRuntime(_options.ConnectionString, _receivingQueue, _eventManager, _myLogger, _settings.MaxConcurrency, _settings.MaxTaskLifeTimeInSeconds, _settings.MillisecondsToSleep);

            _eventManager.StartListening();
            _notificationPublisher.Start();
            return base.StartAsync(cancellationToken);
        }               

        /// <summary>
        /// Stopping is much easier than starting.  Just stops the two listeners.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _notificationPublisher.Stop();
            _eventManager.StopListening();
            return base.StopAsync(cancellationToken);
        }

        
    }
}
