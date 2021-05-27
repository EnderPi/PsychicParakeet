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

namespace EnderPi.Framework.Simulation
{
    public class MultiplyRotate32Simulation : LongRunningTask
    {

        protected override void InitializeInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            
        }

        protected override void StartInternal(CancellationToken token, ServiceProvider provider, int backgroundTaskId, bool persistState)
        {
            var engine = new Sha256();
            IMultiplyRotateDataAccess dataAccess = provider.GetService<IMultiplyRotateDataAccess>();

            while (!token.IsCancellationRequested)
            {
                uint multiplier = engine.Next32() | 1;
                int rotate = engine.NextInt(1, 31);
                if (!dataAccess.RowExists(multiplier, rotate))
                {
                    dataAccess.InsertResult(GetResults(token, multiplier, rotate));
                }
            }            
        }

        private MultiplyRotateResult GetResults(CancellationToken token, uint multiplier, int rotate)
        {
            uint state = 1;
            uint period = 0;
            var _bits = new List<List<byte>>();
            for (int i = 0; i < 32; i++)
            {
                _bits.Add(new List<byte>(10000));
            }

            var linearity = new int[32];
            do
            {
                if (period < 10000)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        _bits[i].Add(Convert.ToByte((state >> i) & 1UL));
                    }
                }
                state = RandomHelper.RotateLeft(state* multiplier, rotate);
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
