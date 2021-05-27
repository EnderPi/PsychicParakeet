using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EnderPi.Framework.DataAccess;
using EnderPi.Framework.Services;
using EnderPi.Framework.Simulation;
using NUnit.Framework;

namespace UnitTest.Framework.Simulation
{
    public class MultiplyRotateTest
    {

        [Test]
        public void TestMultiplyRotate()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            
            ServiceProvider provider = new ServiceProvider();
            int taskId = 1;
            var romulDataAccess = new MultiplyRotateDataAccess(Globals.ConnectionString);
            provider.RegisterService<IMultiplyRotateDataAccess>(romulDataAccess);

            var multiplyrotateTest = new MultiplyRotate32Simulation();
            multiplyrotateTest.Start(source.Token, provider, taskId, false);
            Assert.Pass();
        }
    }
}
