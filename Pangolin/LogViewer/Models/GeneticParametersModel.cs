using System.ComponentModel.DataAnnotations;
using EnderPi.Framework.Simulation.RandomnessTest;
using EnderPi.Framework.Simulation.Genetic;

namespace LogViewer.Models
{
    public class GeneticParametersModel
    {
        [Required]
        public TestLevel Level { set; get; }

        [Required]
        public ConstraintMode Mode { set; get; }

        [Required]
        public GeneticCostMode CostMode { set; get; }
    }
}
