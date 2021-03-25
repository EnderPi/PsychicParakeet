using EnderPi.Framework.Pocos;
using EnderPi.Framework.Simulation.Genetic;

namespace EnderPi.Framework.DataAccess
{
    public interface IGeneticSpecimenDataAccess
    {
        int CreateSpecimen(RngSpecies specimen, int geneticSimulationId);
        void MarkAsConverged(int specimenId);
        RngSpeciesPoco[] ViewConvergedSpecies(int geneticSimulationId);
    }
}