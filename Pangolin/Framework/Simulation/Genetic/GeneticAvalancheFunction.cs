using EnderPi.Framework.Random;
using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic
{
    public class GeneticAvalancheFunction : IGeneticAvalancheFunction
    {
        private ExpressionContext _context;
        private IGenericExpression<ulong> _expressionOutput;

        public GeneticAvalancheFunction(string function)
        {
            _context = new ExpressionContext();
            _context.Imports.AddType(typeof(Math));
            _context.Imports.AddType(typeof(RandomHelper));
            _context.Variables[StateOneNode.Name] = ulong.MaxValue;
            _expressionOutput = _context.CompileGeneric<ulong>(function);
        }

        public ulong Hash(ulong x)
        {
            _context.Variables[StateOneNode.Name] = x;
            return _expressionOutput.Evaluate();
        }
    }
}
