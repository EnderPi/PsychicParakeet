using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EnderPi.Framework.Simulation.RandomnessTest
{
    /// <summary>
    /// An enum for the two types of convergence, relative and absolute.
    /// </summary>
    public enum ConvergenceType { Relative, Absolute };

    /// <summary>
    /// A class to determine when a sequence of doubles has converged.  The class maintains a list of 
    /// the values passed to it, and returns true when the sequence has converged, by Cauchy criteria.
    /// The class can handle both absolute and relative convergence.
    /// </summary>
    public class ConvergenceChecker
    {
        /// <summary>
        /// The number of successive iteration to store.
        /// </summary>
        public int NumberOfIterations { set; get; }
        /// <summary>
        /// The convergence parameter.  Smaller is more strict.
        /// </summary>
        public double Epsilon { set; get; }
        /// <summary>
        /// The type of convergence, relative or absolute.
        /// </summary>
        public ConvergenceType TypeOfConvergence { set; get; }
        /// <summary>
        /// A container which holds all of the values.
        /// </summary>
        public System.Collections.Generic.Queue<double> Values { set; get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="count">The number of values to hold in the queue</param>
        /// <param name="epsilon">Convergence parameter, smaller is stricter</param>
        /// <param name="type">Convergence type, relative or absolute</param>
        public ConvergenceChecker(int count = 4, double epsilon = 0.001, ConvergenceType type = ConvergenceType.Relative)
        {
            NumberOfIterations = count;
            Epsilon = epsilon;
            TypeOfConvergence = type;
            if (NumberOfIterations < 2)
                NumberOfIterations = 2;
            if (Epsilon < 1e-16)
                Epsilon = 1e-16;
            Values = new Queue<double>();
        }

        /// <summary>
        /// Checks whether or not the contained sequence has converged, after adding a parameter.
        /// </summary>
        /// <param name="x">Value to add to the sequence</param>
        /// <returns>True if the sequence has converged, false otherwise</returns>
        public bool IsConverged(double x)
        {
            Values.Enqueue(x);                              //Enqueue up the value
            while (Values.Count > NumberOfIterations)       //Ensure the Queue is not oversized
                Values.Dequeue();
            return IsConverged();        //Check for convergence
        }

        /// <summary>
        /// Check if the contained sequence has converged.
        /// </summary>
        /// <returns>True if the sequence has converged, false otherwise</returns>
        public bool IsConverged()
        {
            if (Values.Count < NumberOfIterations)        //If the queue isn't full, the sequence hasn't converged
                return false;
            switch (TypeOfConvergence)                      //Use the appropriate check for convergence
            {
                case ConvergenceType.Relative:              //if the biggest and smallest are almost equal, return true
                    if (AlmostEqual(Values.Max(), Values.Min(), Epsilon))
                        return true;
                    break;
                case ConvergenceType.Absolute:              //If the difference is less than epsilon, return true
                    if ((Values.Max() - Values.Min()) < Epsilon)
                        return true;
                    break;
            }
            return false;       //if execution reached here, the sequence is NOT converged
        }

        public static bool AlmostEqual(double x, double y, double epsilon = 1e-7, ConvergenceType type = ConvergenceType.Relative)
        {
            if (x == y) return true;        //Always check for actualy equality first
            if (x == 0 || y == 0)           //if either is zero, compare just by absolute
                type = ConvergenceType.Absolute;
            double absoluteMaximum = System.Math.Max(System.Math.Abs(x), System.Math.Abs(y));   //This can't be zero
            switch (type)
            {
                case ConvergenceType.Relative:
                    return (System.Math.Abs(x - y) < System.Math.Abs(epsilon * absoluteMaximum));
                case ConvergenceType.Absolute:
                    return System.Math.Abs(x - y) < epsilon;
                default:
                    throw new Exception("Error in AlmostEqual");
            }
        }

        /// <summary>
        /// Gets the last value in the sequence, which is theoretically the most accurate.
        /// </summary>
        /// <returns>The last value in the sequence</returns>
        public double LastValue() { return Values.Last(); }

        /// <summary>
        /// Returns true if the sequence of successive distances is monotone decreasing, false otherwise.
        /// </summary>
        /// <returns>true for a converging sequence</returns>
        public bool IsConverging()
        {
            double[] x = new double[Values.Count];
            double[] differences = new double[Values.Count - 1];
            int i = 0;
            foreach (var item in Values)
            {
                x[i++] = item;
            }
            for (i = 0; i < x.Count() - 1; i++)
            {
                differences[i] = System.Math.Abs(x[i] - x[i + 1]);
            }
            for (i = 0; i < differences.Count() - 1; i++)
            {
                if (differences[i] < differences[i + 1])
                    return false;
            }
            return true;
        }

    }
}
