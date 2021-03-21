using System;
using Flee.PublicTypes;
using EnderPi.Framework.Simulation.Genetic;

namespace EnderPi.Framework.Random
{
    /// <summary>
    /// Simple one state engine.
    /// </summary>
    [Serializable]
    public class GeneticEngineOneState :Engine
    {
        private ulong _stateOne;
        
        private ExpressionContext _context;
        private IGenericExpression<ulong> _expressionStateOne;
        private IGenericExpression<ulong> _expressionOutput;
        private IGenericExpression<ulong> _expressionSeedOne;
        

        public GeneticEngineOneState(string stateOneExpression, string outputExpression, string seedOneExpression)
        {
            _context = new ExpressionContext();
            _context.Imports.AddType(typeof(Math));
            _context.Variables[StateOneNode.Name] = ulong.MaxValue;
            _context.Variables[SeedNode.Name] = ulong.MaxValue;
            _expressionStateOne = _context.CompileGeneric<ulong>(stateOneExpression);
            _expressionOutput = _context.CompileGeneric<ulong>(outputExpression);
            _expressionSeedOne = _context.CompileGeneric<ulong>(seedOneExpression);            

        }

        public override ulong Next64()
        {
            _stateOne = _expressionStateOne.Evaluate();
            _context.Variables[StateOneNode.Name] = _stateOne;
            return _expressionOutput.Evaluate();
        }

        public override void Seed(ulong seed)
        {
            _context.Variables[SeedNode.Name] = seed;
            _stateOne = _expressionSeedOne.Evaluate();
            _context.Variables[StateOneNode.Name] = _stateOne;            
        }
    }
}
