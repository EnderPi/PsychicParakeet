using System.Threading;
using System.Threading.Tasks;
using EnderPi.Framework.Services;
using EnderPi.Framework.Simulation.Genetic;
using NUnit.Framework;
using Moq;
using EnderPi.Framework.BackgroundWorker;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Caching;

namespace UnitTest.Framework.Simulation.Genetic
{
    /// <summary>
    /// Some simple tests for the Genetic programming.
    /// </summary>
    public class TestBreeding
    {
        [Test]
        public void TestLongRunningTask()
        {
            //var parameters = new GeneticParameters();
            //parameters.Level = EnderPi.Framework.Simulation.RandomnessTest.TestLevel.One;
            //var task = new GeneticRngBreeding(parameters);
            //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            //var provider = new ServiceProvider();
            //IBackgroundTaskManager mock = new Mock<IBackgroundTaskManager>().Object;
            //provider.RegisterService<IBackgroundTaskManager>(mock);

            //var configProvider = new CachedConfigurationProvider(new ConfigurationDataAccess(Globals.ConnectionString), new TimedCache());
            //provider.RegisterService<IConfigurationDataAccess>(configProvider);

            //int taskId = 0;
            //bool persistState = false;
            //Task.Factory.StartNew(() => task.Start(cancellationTokenSource.Token, provider, taskId, persistState));
            //Thread.Sleep(120 * 1000);
            //cancellationTokenSource.Cancel();
            //Thread.Sleep(30 * 1000);
            //var biggest = task.Biggest;
            //var best = task.Best;
            //Assert.IsTrue(best.Fitness > 1);
            
        }

        [Test]
        public void TestLongRunningTaskSync()
        {
            //var parameters = new GeneticParameters();
            //parameters.Level = EnderPi.Framework.Simulation.RandomnessTest.TestLevel.One;
            //var task = new GeneticRngBreeding(parameters);
            //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            //var provider = new ServiceProvider();
            //IBackgroundTaskManager mock = new Mock<IBackgroundTaskManager>().Object;
            //provider.RegisterService<IBackgroundTaskManager>(mock);

            //var configProvider = new CachedConfigurationProvider(new ConfigurationDataAccess(Globals.ConnectionString), new TimedCache());
            //provider.RegisterService<IConfigurationDataAccess>(configProvider);

            //int taskId = 0;
            //bool persistState = false;
            //task.Start(cancellationTokenSource.Token, provider, taskId, persistState);
            //Thread.Sleep(120 * 1000);
            //cancellationTokenSource.Cancel();
            //Thread.Sleep(30 * 1000);
            //var biggest = task.Biggest;
            //var best = task.Best;
            //Assert.IsTrue(best.Fitness > 1);

        }

    }
}
