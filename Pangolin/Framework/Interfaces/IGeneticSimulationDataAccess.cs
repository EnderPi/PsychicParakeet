using EnderPi.Framework.Pocos;
using EnderPi.Framework.Simulation.Genetic;

namespace EnderPi.Framework.DataAccess
{
    public interface IGeneticSimulationDataAccess
    {
        int CreateGeneticSimulation(GeneticParameters geneticSimulation);
        GeneticSimulationPoco GetSimulation(int id);
    }
}