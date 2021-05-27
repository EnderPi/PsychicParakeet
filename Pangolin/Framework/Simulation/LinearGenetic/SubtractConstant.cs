
using System;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    [Serializable]
    public class SubtractConstant :BinaryRegisterConstantCommand
    {
        public SubtractConstant(int registerIndex, ulong constant):base(registerIndex, constant)
        {            
        }
        public override void Execute(ulong[] registers)
        {
            registers[_targetRegisterIndex] -= _constant;
        }

        public override bool IsBackwardsConsistent(ref bool[] nonZeroRegisters)
        {
            if (nonZeroRegisters[_targetRegisterIndex])
            {
                return true;
            }
            return false;
        }

        public override bool IsForwardConsistent(ref bool[] nonZeroRegisters)
        {
            if (_constant != 0)
            {
                nonZeroRegisters[_targetRegisterIndex] = true;
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return $"{Machine8099Grammar.Subtract} {LinearGeneticHelper.GetRegister(_targetRegisterIndex)},{_constant};";
        }
    }
}
