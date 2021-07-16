using EnderPi.Framework.Simulation.Genetic.Nodes32Bit;
using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnderPi.Framework.Random
{
    public class GeneticFeistelEngine : Engine
    {
        private ulong _state;

        private ExpressionContext _context;
        private IGenericExpression<uint> _feistelFunction;
        private int _rounds;
        private uint[] _keys;

        public GeneticFeistelEngine(string expression, int rounds, uint[] keys)
        {
            _rounds = rounds;
            _keys = keys;
            _context = new ExpressionContext();
            _context.Imports.AddType(typeof(Math));
            _context.Imports.AddType(typeof(RandomHelper));
            _context.Variables[StateNode32.Name] = uint.MaxValue;
            _context.Variables[KeyNod32bit.Name] = uint.MaxValue;            
            _feistelFunction = _context.CompileGeneric<uint>(expression);
        }

        public override ulong Next64()
        {            
            uint right = Convert.ToUInt32(_state & UInt32.MaxValue);
            uint left = Convert.ToUInt32(_state >> 32);
            uint temp;
            for (int i=0; i < _rounds; i++)
            {
                temp = right;                               //store the right
                right = left ^ Hash32(right, _keys[i]);   //right is left xor'd with hash of right and key
                left = temp;
            }
            ulong result;
            if ((_rounds & 1) == 1)
            {
                result= (((ulong)right) << 32) | left;
            }
            else
            {
                result = (((ulong)left) << 32) | right;
            }
            _state++;
            return result;
        }

        private uint Hash32(uint x, uint key)
        {
            _context.Variables[StateNode32.Name] = x;
            _context.Variables[KeyNod32bit.Name] = key;
            return _feistelFunction.Evaluate();
        }

        public override void Seed(ulong seed)
        {
            _state = seed;            
        }
    }
}
