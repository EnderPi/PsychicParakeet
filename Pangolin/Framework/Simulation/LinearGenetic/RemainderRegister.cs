using System;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    [Serializable]
    public class RemainderRegister :BinaryRegisterRegisterCommand
    {
        public RemainderRegister(int targetRegisterIndex, int sourceRegisterIndex):base(targetRegisterIndex, sourceRegisterIndex)
        {            
        }
        public override void Execute(ulong[] registers)
        {
            registers[_targetRegisterIndex] %= registers[_sourceRegisterIndex];
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
            return $"{Machine8099Grammar.Remainder} {LinearGeneticHelper.GetRegister(_targetRegisterIndex)},{LinearGeneticHelper.GetRegister(_sourceRegisterIndex)};";
        }
    }
}
