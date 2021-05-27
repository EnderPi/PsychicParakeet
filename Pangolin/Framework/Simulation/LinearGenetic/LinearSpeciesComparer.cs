using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EnderPi.Framework.Simulation.LinearGenetic
{
    public class LinearSpeciesComparer : IComparer<LinearGeneticSpecimen>
    {
        public int Compare([AllowNull] LinearGeneticSpecimen x, [AllowNull] LinearGeneticSpecimen y)
        {
            if (x.Fitness != y.Fitness)
            {
                return x.Fitness.CompareTo(y.Fitness);
            }
            else
            {
                if (x.TestsPassed == y.TestsPassed)
                {
                    return y.ProgramLength.CompareTo(x.ProgramLength);
                }
                else
                {
                    return x.TestsPassed.CompareTo(y.TestsPassed);
                }
            }
        }
    }
}
