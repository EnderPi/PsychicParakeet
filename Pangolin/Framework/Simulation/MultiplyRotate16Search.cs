using EnderPi.Framework.BackgroundWorker;
using System;
using System.Collections.Generic;
using System.Text;
using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Random;
using EnderPi.Framework.Services;
using EnderPi.Framework.Simulation.RandomnessTest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnderPi.Framework.Simulation
{
    public class MultiplyRotate16Search :LongRunningTask
    {
        protected override void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {

        }

        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            IMultiplyRotateDataAccess dataAccess = provider.GetService<IMultiplyRotateDataAccess>();
            //var values = new List<Tuple<int, int>>();
            //for (int i=3; i <= ushort.MaxValue; i+=2)
            //{
            //    for (int j=1; j <16; j++)
            //    {
            //        values.Add(new Tuple<int, int>(i, j));
            //    }
            //}
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            RomulTest[] thingsToTest = dataAccess.GetRowsFor16BitFillIn();
            Parallel.ForEach(thingsToTest, options, (i) =>
            {
                var row = i as RomulTest;

                var rangen = new Romul16((ushort)row.Multiplier, row.Rotate);
                var randomnessTest = new RandomnessSimulation16(TestLevel.Gcd, rangen, 1);
                randomnessTest.Start(token, provider, backgroundTaskId, false);
                row.GcdFitness = randomnessTest.Iterations;
                randomnessTest = new RandomnessSimulation16(TestLevel.Gorilla8, rangen, 1);
                randomnessTest.Start(token, provider, backgroundTaskId, false);
                row.Gorilla8Fitness = randomnessTest.Iterations;
                randomnessTest = new RandomnessSimulation16(TestLevel.Gorilla16, rangen, 1);
                randomnessTest.Start(token, provider, backgroundTaskId, false);
                row.Gorilla16Fitness = randomnessTest.Iterations;
                randomnessTest = new RandomnessSimulation16(TestLevel.Birthday, rangen, 1);
                randomnessTest.Start(token, provider, backgroundTaskId, false);
                row.BirthdayFitness = randomnessTest.Iterations;
                randomnessTest = new RandomnessSimulation16(TestLevel.Maurer16, rangen, 1);
                randomnessTest.Start(token, provider, backgroundTaskId, false);
                row.Maurer16Fitness = randomnessTest.Iterations;
                randomnessTest = new RandomnessSimulation16(TestLevel.Maurer8, rangen, 1);
                randomnessTest.Start(token, provider, backgroundTaskId, false);
                row.Maurer8Fitness = randomnessTest.Iterations;
                if (!token.IsCancellationRequested)
                {
                    dataAccess.UpdateRomul16Row(row);
                }

            });
            
            
        }

        private MultiplyRotateResult GetResults(CancellationToken token, uint multiplier, int rotate)
        {
            uint state = 1;
            uint period = 0;
            var _bits = new List<List<byte>>();
            int size = 10000;
            for (int i = 0; i < 32; i++)
            {
                _bits.Add(new List<byte>(size));
            }

            var linearity = new int[32];
            do
            {
                if (period < size)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        _bits[i].Add(Convert.ToByte((state >> i) & 1UL));
                    }
                }
                state = RandomHelper.RotateLeft(state * multiplier, rotate);
                period++;
            } while ((state != 1) && period != 0);

            for (int i = 0; i < 32; i++)
            {
                linearity[i] = TestHelper.BerlekampMassey(_bits[i].ToArray());
            }


            var result = new MultiplyRotateResult() { Multiplier = multiplier, Rotate = rotate, Period = period, Linearity = linearity };
            return result;
        }

        protected override void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState)
        {

        }
    }
}
