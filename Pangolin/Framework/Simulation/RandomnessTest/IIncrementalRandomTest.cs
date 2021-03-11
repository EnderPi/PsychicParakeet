using EnderPi.Framework.Services;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    public interface IIncrementalRandomTest
    {
        /// <summary>
        /// Processes a random number.
        /// </summary>
        /// <param name="randomNumber"></param>
        public void Process(ulong randomNumber);

        /// <summary>
        /// Calculates the result for the current state of the test.
        /// </summary>
        /// <returns></returns>
        public void CalculateResult(bool detailed);

        public TestResult Result { get; }
        int TestsPassed { get; }

        /// <summary>
        /// Called before the simulation starts, create any large state objects here.
        /// </summary>
        public void Initialize();
        void StoreFinalResults(int backgroundTaskId, ServiceProvider provider, bool persistState);
    }
}
