using EnderPi.Framework.Simulation.Genetic;

namespace EnderPi.Framework.DataAccess
{
    public interface IFeistelSpecimenDataAccess
    {
        int CreateSpecimen(RngSpecies32Feistel specimen, int geneticSimulationId);
        void MarkAsConverged(int specimenId);
    }
}