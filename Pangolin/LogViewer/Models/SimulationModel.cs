using System;
using System.ComponentModel.DataAnnotations;

namespace LogViewer.Models
{
    /// <summary>
    /// POCO for a simulation.  Needs a corresponding data structure tbh.
    /// </summary>
    public class SimulationModel
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { set; get; }

        /// <summary>
        /// The expression to evaluate.  Needs to have an "x"
        /// </summary>
        [Required]
        public string Expression { set; get; }

        /// <summary>
        /// The number of iterations to perform.
        /// </summary>
        [Range(100,1000000)]
        public int Iterations { set; get; }

        /// <summary>
        /// The percentile to report. (Ascending, obvi, between 0 and 100)
        /// </summary>
        [Range(0, 100)]
        public int Percentile { set; get; }
    }
}
