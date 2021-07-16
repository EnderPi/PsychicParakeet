using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using EnderPi.Framework.Simulation.RandomnessTest;
using EnderPi.Framework.Random;


namespace UnitTest.Framework.Simulation.Randomness16Test
{
    public class TestBirthday16
    {
        /// <summary>
        /// So this test creates a "good" engine, then runs a birthday test and verifies it passes.
        /// </summary>
        [Test]
        public void TestGoodRng()
        {
            var engine = new Sha16Bit();
            var birthdayTest = new BirthdayTest16();
            birthdayTest.Initialize();
            for (int i = 0; i < 10000000; i++)
            {
                birthdayTest.Process(engine.Next16());
            }
            birthdayTest.CalculateResult(true);
            var result = birthdayTest.Result;
            Assert.IsTrue(result != TestResult.Fail);
        }

        /// <summary>
        /// So this test creates a "good" engine, then runs a birthday test and verifies it passes.
        /// </summary>
        [Test]
        public void TestRomul()
        {
            var engine = new Romul16();
            var birthdayTest = new BirthdayTest16();
            birthdayTest.Initialize();
            for (int i = 0; i < 10000; i++)
            {
                birthdayTest.Process(engine.Next16());
            }
            birthdayTest.CalculateResult(true);
            var result = birthdayTest.Result;
            Assert.IsTrue(result != TestResult.Fail);
        }


        /// <summary>
        /// So this test creates a "bad" engine, then runs a birthday test and verifies it fails.
        /// </summary>
        [Test]
        public void TestBadRng()
        {
            var engine = new LinearCongruential16();
            var birthdayTest = new BirthdayTest16();
            birthdayTest.Initialize();
            for (int i = 0; i < 10000; i++)
            {
                birthdayTest.Process(engine.Next16());
            }
            birthdayTest.CalculateResult(true);
            var result = birthdayTest.Result;
            Assert.IsTrue(result == TestResult.Fail);
        }
    }
}
