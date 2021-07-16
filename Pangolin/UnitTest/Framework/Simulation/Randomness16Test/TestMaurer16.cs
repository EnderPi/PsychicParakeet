using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnderPi.Framework.Simulation.RandomnessTest;
using EnderPi.Framework.Random;


namespace UnitTest.Framework.Simulation.Randomness16Test
{
    public class TestMaurer16
    {
        [Test]
        public void TestGoodRng()
        {
            var engine = new Sha16Bit();
            var gorillaTest = new MaurerTest16();
            gorillaTest.Initialize();
            for (int i = 0; i < 100000000; i++)
            {
                gorillaTest.Process(engine.Next16());
            }
            gorillaTest.CalculateResult(true);
            var result = gorillaTest.Result;
            Assert.IsTrue(result != TestResult.Fail);
        }

        [Test]
        public void TestRomul()
        {
            var engine = new Romul16();
            var gorillaTest = new MaurerTest16();
            gorillaTest.Initialize();
            for (int i = 0; i < 100000000; i++)
            {
                gorillaTest.Process(engine.Next16());
            }
            gorillaTest.CalculateResult(true);
            var result = gorillaTest.Result;
            Assert.IsTrue(result == TestResult.Fail);
        }


        /// <summary>
        /// So this test creates a "bad" engine, then runs a gorilla test and verifies it fails.
        /// </summary>
        [Test]
        public void TestBadRng()
        {
            var engine = new LinearCongruential16();
            var gorillaTest = new MaurerTest16();
            gorillaTest.Initialize();
            for (int i = 0; i < 100000000; i++)
            {
                gorillaTest.Process(engine.Next16());
            }
            gorillaTest.CalculateResult(true);
            var result = gorillaTest.Result;
            Assert.IsTrue(result == TestResult.Fail);
        }

    }
}
