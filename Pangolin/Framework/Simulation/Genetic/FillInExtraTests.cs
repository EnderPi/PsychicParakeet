using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EnderPi.Framework.Simulation.Genetic
{
    /// <summary>
    /// Runs level 2 and Three on all the converged specimens.
    /// </summary>
    public class FillInExtraTests : LongRunningTask
    {
        protected override void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            
        }

        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            RngSpecies specimen = null;
            do
            {

            } while (specimen != null);

            
            
        }

        protected override void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            
        }
    }
}
