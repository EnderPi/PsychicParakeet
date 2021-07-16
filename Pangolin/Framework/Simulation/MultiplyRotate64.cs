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
    /// <summary>
    /// Figuring out some multiply rotate constants/
    /// </summary>
    [Serializable]
    public class MultiplyRotate64 : LongRunningTask
    {


        protected override void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            //throw new NotImplementedException();
        }

        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            IMultiplyRotateDataAccess dataAccess = provider.GetService<IMultiplyRotateDataAccess>();

            ulong multiplier = 1518061951166098093UL;

            ulong seed = 1;
            AddLevel14Tests(token, provider, backgroundTaskId, persistState);
            //AnalyzeMultipliersAtARotate(provider, backgroundTaskId, dataAccess, seed, 35, token);
            //AnalyzeMultiplierRotateSeedSet(provider, backgroundTaskId, dataAccess, multiplier, 35, token);
            //for (int i=0; i < 32; i++)
            //{
            //    multiplier = Engine.Crypto64();
            //    multiplier = multiplier >> i;
            //    multiplier = multiplier | 1;
            //    AnalyzeMultiplier(provider, backgroundTaskId, dataAccess, seed, multiplier, token);
            //}
                        
            //AnalyzeMultiplierRotateSeedSet(provider, backgroundTaskId, dataAccess, multiplier, 35, token);
            //AnalyzeRotate(provider, backgroundTaskId, dataAccess, 35, token);

        }

        private void AddLevel14Tests(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            IMultiplyRotateDataAccess dataAccess = provider.GetService<IMultiplyRotateDataAccess>();
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            var thingsToTest = dataAccess.GetNextRowThatNeedsLevel14();
            Parallel.For(0,thingsToTest.Length,options, (i) =>
            {
                RomulTest test = thingsToTest[i];

                Romul rangen = new Romul(test.Multiplier, test.Rotate);
                var randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Fourteen, rangen, test.Seed);
                randomnessTest.Start(token, provider, backgroundTaskId, false);
                test.LevelFourteenFitness = randomnessTest.Iterations;
                if (!token.IsCancellationRequested)
                {
                    dataAccess.UpdateLevel14Test(test);
                }


            });
            
        }

        private void AnalyzeRotate(ServiceProvider provider, int backgroundTaskId, IMultiplyRotateDataAccess dataAccess, int rotate, CancellationToken token)
        {
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            Parallel.For(1, 64, options, (i) =>
            {
                ulong multiplier = Engine.Crypto64();
                multiplier = multiplier >> Convert.ToInt32(Engine.Crypto64() & 31);
                multiplier = multiplier | 1;
                ulong seed = Engine.Crypto64();
                if (!dataAccess.RomulExists(multiplier, rotate, seed))
                {
                    RomulTest test = new RomulTest() { Multiplier = multiplier, Rotate = rotate, Seed = seed };
                    Romul rangen = new Romul(multiplier, rotate);
                    var randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Twelve, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelTwelveFitness = randomnessTest.Iterations;
                    if (!token.IsCancellationRequested)
                    {
                        dataAccess.InsertResult(test);
                    }
                }
            });
        }

        private void AnalyzeMultipliersAtARotate(ServiceProvider provider, int backgroundTaskId, IMultiplyRotateDataAccess dataAccess, ulong seed, int rotate, CancellationToken token)
        {
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            Parallel.For(1, 11, options, (i) =>
            {
                ulong multiplier = Engine.Crypto64() | 1UL;
                if (!dataAccess.RomulExists(multiplier, rotate, seed))
                {
                    RomulTest test = new RomulTest() { Multiplier = multiplier, Rotate = rotate, Seed = seed };
                    Romul rangen = new Romul(multiplier, rotate);
                    var randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Twelve, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelTwelveFitness = randomnessTest.Iterations;
                    if (!token.IsCancellationRequested)
                    {
                        dataAccess.InsertResult(test);
                    }
                }
            });
        }

        private void AnalyzeMultiplierRotateSeedSet(ServiceProvider provider, int backgroundTaskId, IMultiplyRotateDataAccess dataAccess, ulong multiplier, int rotate, CancellationToken token)
        {
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            Parallel.For(1, 1001, options, (i) =>
            {
                ulong seed = Engine.Crypto64();
                if (!dataAccess.RomulExists(multiplier, rotate, seed))
                {
                    RomulTest test = new RomulTest() { Multiplier = multiplier, Rotate = rotate, Seed = seed };
                    Romul rangen = new Romul(multiplier, rotate);
                    var randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Three, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelThreeFitness = randomnessTest.Iterations;
                    if (!token.IsCancellationRequested)
                    {
                        dataAccess.InsertResult(test);
                    }
                }
            });
        }

        private static void AnalyzeMultiplier(ServiceProvider provider, int backgroundTaskId, IMultiplyRotateDataAccess dataAccess, ulong seed, ulong multiplier, CancellationToken token)
        {
            ParallelOptions options = new ParallelOptions() { CancellationToken = token, MaxDegreeOfParallelism = 6 };
            Parallel.For(1, 64, options, (rotate)=>            
            {
                //if (!dataAccess.RomulExists(multiplier, rotate, seed))
                //{
                    RomulTest test = new RomulTest() { Multiplier = multiplier, Rotate = rotate, Seed = seed };
                    Romul rangen = new Romul(multiplier, rotate);
                    var randomnessTest = new RandomnessSimulation(RandomnessTest.TestLevel.Three, rangen, seed);
                    randomnessTest.Start(token, provider, backgroundTaskId, false);
                    test.LevelThreeFitness = randomnessTest.Iterations;
                    if (!token.IsCancellationRequested)
                    {
                        dataAccess.InsertResult(test);
                    }
                //}
            });
        }

        protected override void StoreFinalResults(ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            //throw new NotImplementedException();
        }
    }
}
