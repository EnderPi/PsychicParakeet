using EnderPi.Framework.Logging;
using EnderPi.Framework.Services;
using EnderPi.Framework.Simulation;
using EnderPi.Framework.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticWeb.Pages
{
    public partial class Index
    {
        private bool _running = false;

        private CancellationTokenSource _source;

        /// <summary>
        /// Starts the simulation
        ///
        ///</summary>
        public void HandleStartClick()
        {            
            _running = true;
            Thread backgroundThread = new Thread(BackgroundTaskDelegate, 10 * 1024 * 1024);
            backgroundThread.IsBackground = true;
            backgroundThread.Start();            

        }

        public void BackgroundTaskDelegate()
        {            
            MultiplyRotate32Simulation task = new MultiplyRotate32Simulation();
            try
            {
                _source = new CancellationTokenSource();
                var token = _source.Token;
                var provider = new ServiceProvider();
                provider.RegisterService(multiplyRotateDataAccess);
                provider.RegisterService(logger);                
                task.Start(token, provider, 0, false);                
            }
            catch (Exception ex)
            {
                var details = new LogDetails();
                details.AddDetail("Exception", ex.ToString());
                logger.Log("Error Running Simulation!", LoggingLevel.Error, details);
            }
            finally
            {
                _source.Dispose();
                _running = false;
            }
        }

        /// <summary>
        /// Stops the simulation
        ///</summary>
        public void HandleStopClick()
        {
            Threading.ExecuteWithoutThrowing(() => _source.Cancel());
        }

    }
}
