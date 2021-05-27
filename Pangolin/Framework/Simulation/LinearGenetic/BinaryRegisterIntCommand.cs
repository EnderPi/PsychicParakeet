using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    [Serializable]
    public abstract class BinaryRegisterIntCommand : Command8099
    {
        protected int _targetRegisterIndex;
        protected int _constant;

        public int Constant { set { _constant = value; } get { return _constant; } }
        public BinaryRegisterIntCommand(int targetRegisterIndex, int constant)
        {
            _targetRegisterIndex = targetRegisterIndex;
            _constant = constant;
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
