using System;
using EnderPi.Framework.Simulation.Genetic;
using Flee.PublicTypes;

namespace EnderPi.Framework.Random
{
    public class GeneticEngine : Engine
    {
        private ulong _stateOne;
        private ulong _stateTwo;

        private ExpressionContext _context;
        private IGenericExpression<ulong> _expressionStateOne;
        private IGenericExpression<ulong> _expressionStateTwo;
        private IGenericExpression<ulong> _expressionOutput;
        private IGenericExpression<ulong> _expressionSeedOne;
        private IGenericExpression<ulong> _expressionSeedTwo;


        public GeneticEngine(string stateOneExpression, string stateTwoExpression, string outputExpression, string seedOneExpression, string seedTwoExpression)
        {
            _context = new ExpressionContext();
            _context.Imports.AddType(typeof(Math));
            _context.Variables[StateOneNode.Name] = ulong.MaxValue;
            _context.Variables[StateTwoNode.Name] = ulong.MaxValue;
            _context.Variables[SeedNode.Name] = ulong.MaxValue;
            _expressionStateOne = _context.CompileGeneric<ulong>(stateOneExpression);
            _expressionStateTwo = _context.CompileGeneric<ulong>(stateTwoExpression);
            _expressionOutput = _context.CompileGeneric<ulong>(outputExpression);
            _expressionSeedOne = _context.CompileGeneric<ulong>(seedOneExpression);
            _expressionSeedTwo = _context.CompileGeneric<ulong>(seedTwoExpression);

        }

        public override ulong Next64()
        {
            _stateOne = _expressionStateOne.Evaluate();
            _stateTwo = _expressionStateTwo.Evaluate();
            _context.Variables[StateOneNode.Name] = _stateOne;
            _context.Variables[StateTwoNode.Name] = _stateTwo;
            return _expressionOutput.Evaluate();            
        }

        public override void Seed(ulong seed)
        {
            _context.Variables[SeedNode.Name] = seed;
            _stateOne = _expressionSeedOne.Evaluate();
            _stateTwo = _expressionSeedTwo.Evaluate();
            _context.Variables[StateOneNode.Name] = _stateOne;
            _context.Variables[StateTwoNode.Name] = _stateTwo;
        }
    }
}
