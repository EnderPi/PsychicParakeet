using System;
using System.Threading;

namespace EnderPi.Framework.Threading
{
    /// <summary>
    /// Used by the throttled task listener framework so that it can forcibly kill tasks.
    /// </summary>
    public class CancellableTask
    {
        public Thread TaskThread { set; get; }
        public DateTime TimeStarted { set; get; }

        public CancellableTask() { }

        public int LifeTimeInSeconds { set; get; }

        public void SetAction(Action action)
        {
            TaskThread = new Thread(() => action());
            TaskThread.IsBackground = true;
        }

        public void Start()
        {
            TimeStarted = DateTime.Now;
            TaskThread.Start();
        }

        public void Cancel()
        {
            TaskThread.Abort();
        }

    }
}
