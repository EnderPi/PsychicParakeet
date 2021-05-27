﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    public static class Machine8099ProgramCleaner
    {
        

        /// <summary>
        /// Cleans the program, removing statements that don't do anything.
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static List<Command8099> CleanProgram(Command8099[] program)
        {
            List<Command8099> reducedProgram = new List<Command8099>(program.Length);
            bool[] IsNonZero = new bool[8] { true, true, false, false, false, false, false, false};
            for(int i=0; i <program.Length; i++)
            {
                if (program[i].IsForwardConsistent(ref IsNonZero))
                {
                    reducedProgram.Add(program[i]);
                }
            }

            bool[] affectsStateOrOutput = new bool[8] { true, true, false, false, false, false, false, true };
            var forwardConsistentProgram = reducedProgram.ToArray();
            var reducedProgram2 = new List<Command8099>(forwardConsistentProgram.Length);
            for (int j=forwardConsistentProgram.Length-1; j >=0; j--)
            {
                if (forwardConsistentProgram[j].IsBackwardsConsistent(ref affectsStateOrOutput))
                {
                    reducedProgram2.Add(forwardConsistentProgram[j]);
                }
                //so we walk backwards, and a command is only valid if it affects the forward chain - move dx, 102; move op, s1; - the first is not valid.
                //we make a list of all those commands, then we reverse them
            }
                        

            reducedProgram2.Reverse();
            return reducedProgram2;            
        }                
    }
}
