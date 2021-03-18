using System;

namespace EnderPi.Framework.Threading
{
    /// <summary>
    /// POCO for passing parameters into the throttled task processor.
    /// </summary>
    [Serializable]
    public class ThrottledTaskProcessorParameters
    {
        public ThrottledTaskProcessorParameters(int concurrency, int maxTaskLifetimeInSeconds, int millisecondsToSleep, int secondsBetweenHousekeeping, bool cancelTasks)
        {
            Concurrency = concurrency;
            MaxtaskLifetimeInSeconds = maxTaskLifetimeInSeconds;
            MillisecondsToSleep = millisecondsToSleep;
            SecondsBetweenHousekeeping = secondsBetweenHousekeeping;
            CancelTasks = cancelTasks;
        }
        public int Concurrency { set; get; }
        public int MaxtaskLifetimeInSeconds { set; get; }
        public int MillisecondsToSleep { set; get; }
        public int SecondsBetweenHousekeeping { set; get; }
        public bool CancelTasks { set; get; }
    }
}
