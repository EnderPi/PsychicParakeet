using EnderPi.Framework.Pocos;
using System.Collections.Generic;

namespace EnderPi.Framework.BackgroundWorker
{
    public interface IBackgroundTaskManager
    {
        LongRunningTask DeserializeTask(string xml);
        RunningSimulationReference GetNextTask(List<RunningSimulationReference> currentlyRunningSimulations);
        SimulationPoco GetTask(int simulationId);
        void MarkSimulationAsCanceled(int simulationId);
        void MarkSimulationComplete(int simulationId);
        void ReportProgress(int backgroundTaskId, double percentComplete);
        void SaveIfNecessary(LongRunningTask simulation, int backgroundTaskId);
        void SaveTask(LongRunningTask simulation, int backgroundTaskId);
        string Serialize(LongRunningTask task);
        void SubmitBackgroundTask(LongRunningTask task);
    }
}