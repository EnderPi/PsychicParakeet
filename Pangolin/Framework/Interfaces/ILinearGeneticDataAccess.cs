using EnderPi.Framework.Simulation.LinearGenetic;

namespace EnderPi.Framework.DataAccess
{
    public interface ILinearGeneticDataAccess
    {
        int CreateSpecimen(LinearGeneticSpecimen specimen, int geneticSimulationId);
        void MarkAsConverged(int specimenId);
    }
}