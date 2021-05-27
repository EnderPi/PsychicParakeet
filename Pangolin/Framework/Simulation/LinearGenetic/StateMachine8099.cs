using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    public class StateMachine8099
    {
        private ulong[] _registers;

        public StateMachine8099()
        {
            _registers = new ulong[7];
        }

        public void ExecuteProgram(IEnumerable<Command8099> program)
        {
            foreach(var command in program)
            {
                command.Execute(_registers);
            }
        }
    }
}
