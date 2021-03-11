using System.Threading;

namespace EnderPi.Framework.BackgroundWorker
{
    /// <summary>
    /// POCO held onto by the BackgroundTaskRuntime.  Holds meaningful data related to the simulation, plus a reference to the thread and the cancellation token.
    /// </summary>
    public class RunningSimulationReference
    {
        public int SimulationId { set; get; }
        public CancellationTokenSource CancellationSource { set; get; }

        public LongRunningTask Task {set;get;}
    }
}
