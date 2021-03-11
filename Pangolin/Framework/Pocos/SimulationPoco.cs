using System;

namespace EnderPi.Framework.Pocos
{
    /// <summary>
    /// POCO for the Simulations.Simulation table
    /// </summary>
    public class SimulationPoco
    {
        public int Id { set; get; }
        public string SaveFile { set; get; }
        public string Description { set; get; }
        public DateTime? TimeLastSaved { set; get; }
        public string SimulationObject { set; get; }
        public bool IsRunning { set; get; }
        public bool IsFinished { set; get; }
        public double PercentComplete { set; get; }
        public int Priority { set; get; }
        public bool IsCancelled { set; get; }
        public DateTime? TimeStarted { set; get; }        
        public double PercentCompleteWhenStarted { set; get; }
        public DateTime? EstimatedFinishTime { set; get; }
        public DateTime? TimeCompleted { set; get; }
    }
}
