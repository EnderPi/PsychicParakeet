using EnderPi.Framework.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    /// <summary>
    /// A random number engine based on custom assembly.
    /// </summary>
    [Serializable]
    public class LinearGeneticEngine : Engine
    {
        private Command8099[] _generatorProgram;

        private Command8099[] _seedProgram;

        //State is in the first one or two, output is in the last.
        private ulong[] _registers;

        private const int _registerSize = 8;

        public LinearGeneticEngine(IEnumerable<Command8099> generatorProgram, IEnumerable<Command8099> seedProgram)
        {
            if (generatorProgram == null)
            {
                throw new ArgumentNullException(nameof(generatorProgram));
            }
            if (seedProgram == null)
            {
                seedProgram = new Command8099[0];
            }
            _generatorProgram = RemoveIntrons(generatorProgram);
            _seedProgram = RemoveIntrons(seedProgram);
            _registers = new ulong[_registerSize];
        }

        public Command8099[] RemoveIntrons(IEnumerable<Command8099> program)
        {
            List<Command8099> commands = new List<Command8099>();
            commands.AddRange(program.Where(x=>!(x is IntronCommand)));
            return commands.ToArray();
        }

        public override ulong Next64()
        {            
            ClearNonStateRegisters();
            ExecuteProgram(_generatorProgram);
            return _registers[_registerSize - 1];
        }

        private void ClearNonStateRegisters()
        {
            for (int i = 2; i < _registerSize; i++)
            {
                _registers[i] = 0;
            }
        }

        public void ExecuteProgram(Command8099[] program)
        {
            for (int i = 0; i < program.Length; i++)
            {
                program[i].Execute(_registers);
            }
        }

        public override void Seed(ulong seed)
        {
            _registers[0] = seed;
            _registers[1] = seed;
            ClearNonStateRegisters();
            ExecuteProgram(_seedProgram);
        }
    }
}
