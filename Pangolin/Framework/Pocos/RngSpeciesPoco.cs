using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Pocos
{
    /// <summary>
    /// DTO for rng species
    /// </summary>
    public class RngSpeciesPoco
    {
		public int Id { set; get; }
		public int GeneticSimulationId { set; get; }
		public int Generation { set; get; }
		public int Fitness { set; get; }
		public ulong Seed { set; get; }
		public bool Converged { set; get; }
		public int NumberOfNodes { set; get; }
		public double Cost { set; get; }
		public string StateOneExpression { set; get; }
		public string StateTwoExpression { set; get; }
		public string OutputExpression { set; get; }
		public string SeedOneExpression { set; get; }
		public string SeedTwoExpression { set; get; }

		public string StateOneExpressionPretty { set; get; }
		public string StateTwoExpressionPretty { set; get; }
		public string OutputExpressionPretty { set; get; }
		public string SeedOneExpressionPretty { set; get; }
		public string SeedTwoExpressionPretty { set; get; }

	}
}
