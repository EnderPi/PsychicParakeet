using System;
using EnderPi.Framework.Random;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    /// <summary>
    /// ROTR AX, BX
    /// </summary>
    [Serializable]
    public class RotateRightRegister :BinaryRegisterRegisterCommand
    {
        public RotateRightRegister(int registerIndex, int sourceIndex):base(registerIndex, sourceIndex)
        {            
        }

        public override void Execute(ulong[] registers)
        {
            registers[_targetRegisterIndex] = RandomHelper.RotateRight(registers[_targetRegisterIndex], Convert.ToInt32(registers[_sourceRegisterIndex] & 63UL));
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
            if (nonZeroRegisters[_targetRegisterIndex] && nonZeroRegisters[_sourceRegisterIndex])
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{Machine8099Grammar.RotateRight} {LinearGeneticHelper.GetRegister(_targetRegisterIndex)},{LinearGeneticHelper.GetRegister(_sourceRegisterIndex)};";
        }
    }
}
