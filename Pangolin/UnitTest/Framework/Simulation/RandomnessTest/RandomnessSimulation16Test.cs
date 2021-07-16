using NUnit.Framework;
using EnderPi.Framework.Simulation;
using EnderPi.Framework.Simulation.RandomnessTest;
using EnderPi.Framework.Random;
using EnderPi.Framework.Services;
using EnderPi.Framework.BackgroundWorker;
using Moq;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UnitTest.Framework.Simulation.RandomnessTest
{
    public class RandomnessSimulation16Test
    {
        /// <summary>
        /// So this test creates a "good" engine, then runs a gorilla test and verifies it passes.
        /// </summary>
        [Test]
        public void TestGoodRng()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            var engine = new Sha16Bit();
            ServiceProvider provider = new ServiceProvider();
            int taskId = 1;
            IBackgroundTaskManager mock = new Mock<IBackgroundTaskManager>().Object;
            provider.RegisterService<IBackgroundTaskManager>(mock);

            var randomnessSimulation = new RandomnessSimulation16(TestLevel.One, engine, 12345);
            randomnessSimulation.Start(source.Token, provider, taskId, false);
            var result = randomnessSimulation.Result;
            Assert.IsTrue(result != TestResult.Fail);
        }
                

        /// <summary>
        /// So this test creates a "bad" engine, then runs a test and verifies it fails.
        /// </summary>
        [Test]
        public void TestBadRng()
        {
            var engine = new LinearCongruential16();
            CancellationTokenSource source = new CancellationTokenSource();
            ServiceProvider provider = new ServiceProvider();
            int taskId = 1;
            IBackgroundTaskManager mock = new Mock<IBackgroundTaskManager>().Object;
            provider.RegisterService<IBackgroundTaskManager>(mock);

            var randomnessSimulation = new RandomnessSimulation16(TestLevel.One, engine, 12345);
            randomnessSimulation.Start(source.Token, provider, taskId, false);
            var result = randomnessSimulation.Result;
            Assert.IsTrue(result == TestResult.Fail);
        }
    }
}
