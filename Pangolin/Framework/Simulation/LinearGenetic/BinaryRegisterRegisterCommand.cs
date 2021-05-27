using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    [Serializable]
    public abstract class BinaryRegisterRegisterCommand :Command8099
    {
        protected int _targetRegisterIndex;
        protected int _sourceRegisterIndex;

        public BinaryRegisterRegisterCommand(int targetRegisterIndex, int sourceRegisterIndex)
        {
            _targetRegisterIndex = targetRegisterIndex;
            _sourceRegisterIndex = sourceRegisterIndex;
        }

        public override bool AffectsOutput()
        {
            return _targetRegisterIndex == (int)Machine8099Registers.OP;
        }

        public override bool AffectsState()
        {
            return _targetRegisterIndex == (int)Machine8099Registers.S1 || _targetRegisterIndex == (int)Machine8099Registers.S2;
        }

    }
}
