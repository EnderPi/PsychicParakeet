using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    /// <summary>
    /// XOR's a register with another register.
    /// </summary>
    [Serializable]
    public class XorRegister :BinaryRegisterRegisterCommand
    {
        public XorRegister(int targetRegisterIndex, int sourceRegisterIndex):base(targetRegisterIndex, sourceRegisterIndex)
        {            
        }

        public override void Execute(ulong[] registers)
        {
            registers[_targetRegisterIndex] = registers[_targetRegisterIndex] ^ registers[_sourceRegisterIndex];
        }

        public override bool IsBackwardsConsistent(ref bool[] registersThatAffectOutputorState)
        {
            if (registersThatAffectOutputorState[_targetRegisterIndex])
            {
                registersThatAffectOutputorState[_sourceRegisterIndex] = true;
                return true;
            }
            return false;
        }

        public override bool IsForwardConsistent(ref bool[] nonZeroRegisters)
        {
            if (nonZeroRegisters[_sourceRegisterIndex])
            {
                nonZeroRegisters[_targetRegisterIndex] = true;
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return $"{Machine8099Grammar.Xor} {LinearGeneticHelper.GetRegister(_targetRegisterIndex)},{LinearGeneticHelper.GetRegister(_sourceRegisterIndex)};";
        }

    }
}
