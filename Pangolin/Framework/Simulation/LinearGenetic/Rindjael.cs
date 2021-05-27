using EnderPi.Framework.Random;
using EnderPi.Framework.Simulation.Genetic.Nodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    [Serializable]
    public class Rindjael :Command8099
    {
        private int _registerIndex;

        public Rindjael(int registerIndex)
        {
            _registerIndex = registerIndex;
        }

        public override bool AffectsOutput()
        {
            return _registerIndex == (int)Machine8099Registers.OP;
        }

        public override bool AffectsState()
        {
            return _registerIndex == (int)Machine8099Registers.S1 || _registerIndex == (int)Machine8099Registers.S2;
        }

        public override void Execute(ulong[] registers)
        {
            registers[_registerIndex] = RandomHelper.Rindjael(registers[_registerIndex]);
        }

        public override bool IsBackwardsConsistent(ref bool[] registersThatAffectOutputorState)
        {
            if (registersThatAffectOutputorState[_registerIndex])
            {
                return true;
            }
            return false;
        }

        public override bool IsForwardConsistent(ref bool[] nonZeroRegisters)
        {
            //always always effects the target.
            nonZeroRegisters[_registerIndex] = true;
            return true;
        }

        public override string ToString()
        {
            return $"{Machine8099Grammar.Rindjael} {LinearGeneticHelper.GetRegister(_registerIndex)};";
        }
    }
}
