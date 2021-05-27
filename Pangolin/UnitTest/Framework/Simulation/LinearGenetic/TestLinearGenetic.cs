using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using EnderPi.Framework.Simulation.Genetic;
using NUnit.Framework;
using EnderPi.Framework.Random;
using EnderPi.Framework.Simulation.LinearGenetic;

namespace UnitTest.Framework.Simulation.LinearGenetic
{
    public class TestLinearGenetic
    {
        /// <summary>
        /// Creates an LCG from an RNG species, verifies it matches the LCG built normally.
        /// </summary>
        [Test]
        public void TestLcg()
        {
            List<Command8099> program = new List<Command8099>();
            program.Add(new MultiplyConstant(0, 3935559000370003845));
            program.Add(new AddConstant(0, 2691343689449507681));
            program.Add(new MoveRegister(7, 0));

            var engine = new LinearGeneticEngine(program.ToArray(), new Command8099[0]);
            var lcgGen = new LinearCongruential();

            ulong seed = 5;
            lcgGen.Seed(seed);
            engine.Seed(seed);

            for (int i = 0; i < 10; i++)
            {
                Assert.IsTrue(lcgGen.Next64() == engine.Next64());
            }
        }

        [Test]
        public void SpeedTestLcg()
        {
            List<Command8099> program = new List<Command8099>();
            program.Add(new MultiplyConstant(0, 3935559000370003845));
            program.Add(new AddConstant(0, 2691343689449507681));
            program.Add(new MoveRegister(7, 0));
            for (int i = 0; i < 97; i++)
            {
                program.Add(new IntronCommand());
            }

            var engine = new LinearGeneticEngine(program.ToArray(),new Command8099[0]);

            ulong seed = 5;
            engine.Seed(seed);

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                engine.Next64();
            }
            stopwatch.Stop();

            var lcgGen = new LinearCongruential();
            var stopwatch2 = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                lcgGen.Next64();
            }
            stopwatch2.Stop();



            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000000);
            Assert.IsTrue(stopwatch2.ElapsedMilliseconds < 1000000);
        }


        [Test]
        public void TestCryptRangen()
        {

        }

    }
}
