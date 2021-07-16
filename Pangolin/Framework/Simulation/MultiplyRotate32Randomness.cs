using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Pocos;
using EnderPi.Framework.Random;
using EnderPi.Framework.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EnderPi.Framework.Simulation
{
    [Serializable]
    public class MultiplyRotate32Randomness :LongRunningTask
    {
        private static int[] _randomRotateCandidates = new int[] { 7, 11, 13, 19, 21 };
        protected override void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            //throw new NotImplementedException();
        }

        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            IMultiplyRotateDataAccess dataAccess = provider.GetService<IMultiplyRotateDataAccess>();

            uint multiplier = 699192619;

            ulong seed = 1;
            AnalyzeMultipliersAtARotate(provider, backgroundTaskId, dataAccess, seed, 13, token);
            //AnalyzeMultiplierRotateSeedSet(provider, backgroundTaskId, dataAccess, multiplier, 35, token);
            //AnalyzeMultiplier(provider, backgroundTaskId, dataAccess, seed, multiplier, token);
            //AnalyzeMultiplier(provider, backgroundTaskId, dataAccess, seed, multiplier + 2, token);
            //multiplier = 2840963435;
            //AnalyzeMultiplier(provider, backgroundTaskId, dataAccess, seed, multiplier, token);
            //AnalyzeMultiplier(provider, backgroundTaskId, dataAccess, seed, multiplier + 2, token);
            //AnalyzeMultiplierRotateSeedSet(provider, backgroundTaskId, dataAccess, multiplier, 35, token);
            //AnalyzeRotate(provider, backgroundTaskId, dataAccess, 35, token);

        }

        private void AnalyzeRotate(ServiceProvider provider, int backgroundTaskId, IMultiplyRotateDataAccess dataAccess, int rotate, CancellationToken token)
        {
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            Parallel.For(1, 51, options, (i) =>
            {
                uint multiplier = (uint)(Engine.Crypto64() & Convert.ToUInt64(UInt32.MaxValue));
                multiplier = multiplier >> Convert.ToInt32(Engine.Crypto64() & 15);
                multiplier = multiplier | 1;
                ulong seed = Engine.Crypto64();
                if (!dataAccess.RomulExists32(multiplier, rotate, seed))
                {
                    RomulTest test = new RomulTest() { Multiplier = multiplier, Rotate = rotate, Seed = seed };
                    var rangen = new Romul32(multiplier, rotate);
                    var randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Twelve, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelTwelveFitness = randomnessTest.Iterations;
                    if (!token.IsCancellationRequested)
                    {
                        dataAccess.InsertResult32(test);
                    }
                }
            });
        }

        private void AnalyzeMultipliersAtARotate(ServiceProvider provider, int backgroundTaskId, IMultiplyRotateDataAccess dataAccess, ulong seed, int rotate, CancellationToken token)
        {
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            Parallel.For(1, 41, options, (i) =>
            {
                uint multiplier = (uint)(Engine.Crypto64() & Convert.ToUInt64(UInt32.MaxValue));
                multiplier = multiplier >> Convert.ToInt32(Engine.Crypto64() & 15);
                multiplier = multiplier | 1;
                if (!dataAccess.RomulExists32(multiplier, rotate, seed))
                {
                    RomulTest test = new RomulTest() { Multiplier = multiplier, Rotate = rotate, Seed = seed };
                    Romul32 rangen = new Romul32(multiplier, rotate);
                    var randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Thirteen, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelThirteenFitness = randomnessTest.Iterations;
                    randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.One, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelOneFitness = randomnessTest.Iterations;
                    randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Two, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelTwoFitness = randomnessTest.Iterations;
                    randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Three, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelThreeFitness = randomnessTest.Iterations;
                    randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Twelve, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelTwelveFitness = randomnessTest.Iterations;
                    if (!token.IsCancellationRequested)
                    {
                        dataAccess.InsertResult32(test);
                    }
                }
            });
        }

        private void AnalyzeMultiplierRotateSeedSet(ServiceProvider provider, int backgroundTaskId, IMultiplyRotateDataAccess dataAccess, uint multiplier, int rotate, CancellationToken token)
        {
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            Parallel.For(1, 1001, options, (i) =>
            {
                ulong seed = Engine.Crypto64();
                if (!dataAccess.RomulExists32(multiplier, rotate, seed))
                {
                    RomulTest test = new RomulTest() { Multiplier = multiplier, Rotate = rotate, Seed = seed };
                    var rangen = new Romul32(multiplier, rotate);
                    var randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Three, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelThreeFitness = randomnessTest.Iterations;
                    if (!token.IsCancellationRequested)
                    {
                        dataAccess.InsertResult32(test);
                    }
                }
            });
        }

        private static void AnalyzeMultiplier(ServiceProvider provider, int backgroundTaskId, IMultiplyRotateDataAccess dataAccess, ulong seed, uint multiplier, CancellationToken token)
        {
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            Parallel.ForEach(_randomRotateCandidates, options, (rotate) =>
            {
                RomulTest test = new RomulTest() { Multiplier = multiplier, Rotate = rotate, Seed = seed };
                Romul32 rangen = new Romul32(multiplier, rotate);
                var randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Thirteen, rangen, seed);
                randomnessTest.Start(token, provider, backgroundTaskId, false);
                test.LevelThirteenFitness = randomnessTest.Iterations;
                if (!token.IsCancellationRequested)
                {
                    dataAccess.InsertResult32(test);
                }
            });
        }

        protected override void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            //throw new NotImplementedException();
        }

    }
}
