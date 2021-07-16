using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnderPi.Framework.Simulation.RandomnessTest;
using EnderPi.Framework.Random;

namespace UnitTest.Framework.Simulation.RandomnessTest
{
    public class TestEngine
    {
        /// <summary>
        /// Tests the transform on the engine class.
        /// </summary>
        [Test]
        public void TestEngineLimits()
        {
            Constant c1 = new Constant();
            c1.Seed(0);
            Assert.AreEqual(0, c1.NextDouble());
            Assert.AreEqual(0, c1.NextDoubleInclusive());
            Assert.AreNotEqual(0, c1.NextDoubleExclusive());
            c1.Seed(ulong.MaxValue);
            Assert.AreNotEqual(1, c1.NextDouble());
            Assert.AreEqual(1, c1.NextDoubleInclusive());
            Assert.AreNotEqual(0, c1.NextDoubleExclusive());

        }

    }
}
