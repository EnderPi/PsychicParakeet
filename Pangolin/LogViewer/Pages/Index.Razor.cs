using EnderPi.Framework.DataAccess;
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
        private bool _running2 = false;

        private CancellationTokenSource _source;
        private CancellationTokenSource _source2;

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

        public void HandleStartClick2()
        {
            _running2 = true;
            Thread backgroundThread = new Thread(BackgroundTaskDelegate2, 10 * 1024 * 1024);
            backgroundThread.IsBackground = true;
            backgroundThread.Start();

        }

        public void BackgroundTaskDelegate()
        {            
            var task = new MultiplyRotate16Search();
            try
            {
                _source = new CancellationTokenSource();
                var token = _source.Token;
                var provider = new ServiceProvider();
                provider.RegisterService(multiplyRotateDataAccess);
                provider.RegisterService(logger);
                provider.RegisterService(backgroundTaskManager);
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
                InvokeAsync(() => StateHasChanged());
            }
        }

        public void BackgroundTaskDelegate2()
        {
            var task = new MultiplyRotate64();
            try
            {
                _source2 = new CancellationTokenSource();
                var token = _source2.Token;
                var provider = new ServiceProvider();
                provider.RegisterService<IMultiplyRotateDataAccess>(multiplyRotateDataAccess);
                provider.RegisterService(logger);
                provider.RegisterService(backgroundTaskManager);
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
                _source2.Dispose();
                _running2 = false;
                InvokeAsync(() => StateHasChanged());
            }
        }

        /// <summary>
        /// Stops the simulation
        ///</summary>
        public void HandleStopClick()
        {
            Threading.ExecuteWithoutThrowing(() => _source.Cancel());
        }

        public void HandleStopClick2()
        {
            Threading.ExecuteWithoutThrowing(() => _source2.Cancel());
        }

    }
}
