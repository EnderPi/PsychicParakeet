using System;
using System.Collections.Generic;
using System.Text;
using EnderPi.Framework.Services;


namespace EnderPi.Framework.Simulation.RandomnessTest
{
    interface IIncremental32RandomTest
    {
        /// <summary>
        /// Processes a random number.
        /// </summary>
        /// <param name="randomNumber"></param>
        public void Process(uint randomNumber);

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
