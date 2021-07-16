using EnderPi.Framework.Random;
using EnderPi.Framework.Simulation.Genetic.Nodes32Bit;
using EnderPi.Framework.Simulation.RandomnessTest;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic
{
    /// <summary>
    /// 32-bit version of RNG Species for making Feistel functions.
    /// </summary>
    [Serializable]
    public class RngSpecies32Feistel
    {
        private TreeNode _outputRoot;
        private string _imageString;
        private uint[] _keys;
        private int _rounds;
        
        /// <summary>
        /// How fit this specimen is.  Higher values are better.
        /// </summary>
        public double Fitness { set; get; }

        public AvalancheResult AvalancheResults { set; get; }

        /// <summary>
        /// The generation this species was created during.
        /// </summary>
        public int Generation { set; get; }

        public int Rounds { get { return _rounds; } }

        public uint[] Keys { get { return _keys; } }

        /// <summary>
        /// The specimen ID, which is tied to the record in the underlying table.
        /// </summary>
        public int SpecimenId { set; get; }

        /// <summary>
        /// The number of tests passed.
        /// </summary>
        public int TestsPassed { set; get; }

        public ulong Seed { set; get; }

        public DateTime Birthday { set; get; }

        public string Name { set; get; }

        /// <summary>
        /// Values of all constants.
        /// </summary>
        private List<Tuple<uint, string>> _constantValue;

        public List<Tuple<uint, string>> ConstantNameList { get { return _constantValue; } }

        public string ConstantNames
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (_constantValue != null)
                {
                    foreach (var val in _constantValue)
                    {
                        sb.AppendLine($"{val.Item2} = {val.Item1.ToString("N0")}");
                    }
                }
                return sb.ToString();
            }
        }

        public string ImageString
        {
            get
            {
                if (_imageString == null)
                {
                    _imageString = GetImageString();
                }
                return _imageString;
            }
        }


        public RngSpecies32Feistel():this(2, new uint[] { 1,2})
        { }

        /// <summary>
        /// Default constructor, generates very boring species with zero state transition and just the addition of the two states as output.
        /// </summary>
        public RngSpecies32Feistel(int rounds, uint[] keys) 
        {
            AvalancheResults = new AvalancheResult();
            _rounds = rounds;
            _keys = keys;
                        
            _outputRoot = new IntronNode(new AdditionNode(new KeyNod32bit(), new StateNode32()));
            
            Birthday = DateTime.Now;
        }

        public TreeNode GetTreeRoot()
        {
            return _outputRoot;            
        }

        public void SetNodeAge(int age)
        {
            var root = GetTreeRoot();
            root.GenerationOfOrigin = age;
            foreach (var descendant in root.GetDescendants())
            {
                descendant.GenerationOfOrigin = age;
            }
        }

        public double GetAverageNodeAge()
        {
            double average = 0;
            int count = 0;

            var root = GetTreeRoot();
            foreach (var descendant in root.GetDescendants())
            {
                average += descendant.GenerationOfOrigin;
                count++;
            }

            if (count != 0)
            {
                return average / count;
            }
            else
            {
                return 0;
            }
        }

        public RngSpecies32Feistel(TreeNode output)
        {
            _outputRoot = new IntronNode(output);            
        }

        public string GetImageString()
        {
            var bitmap = GetImageBitMap();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Gif);
            var base64Data = Convert.ToBase64String(ms.ToArray());
            return "data:image/gif;base64," + base64Data;
        }

        public Bitmap GetImageBitMap()
        {
            var engine = GetEngine() as Engine;
            engine.Seed(Seed);
            return engine.GetBitMap(4096);
        }              

        internal bool IsValid(out string errors)
        {
            StringBuilder sb = new StringBuilder();
            bool outputHasStateOne = _outputRoot.GetDescendants().Any(x => x is KeyNod32bit);
            bool outputHasStateTwo = _outputRoot.GetDescendants().Any(x => x is StateNode32);
            if (!outputHasStateOne || !outputHasStateTwo)
            {
                sb.AppendLine("Output lacks state or key.");
            }
            errors = sb.ToString();
            if (string.IsNullOrWhiteSpace(errors))
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Gets the total node count from all three trees.
        /// </summary>
        public int NodeCount
        {
            get
            {
                return _outputRoot.GetTotalNodeCount();
            }
        }

        public double TotalCost
        {
            get
            {
                return _outputRoot.GetTotalCost();
            }
        }

        /// <summary>
        /// Get an Engine from the generator.  Need to parse the tree to make a string, then use FLEE to get a function, then return the resultant engine?
        /// </summary>
        /// <returns></returns>
        public IEngine GetEngine()
        {
            return new GeneticFeistelEngine(_outputRoot.Evaluate(), _rounds, _keys);
        }

        /// <summary>
        /// Names all the constants so it can be displayed appropriately.
        /// </summary>
        public void NameConstants()
        {
            int counter = 1;
            _constantValue = new List<Tuple<uint, string>>();

            var tree = GetTreeRoot();
            var descendantConstants = tree.GetDescendants().Distinct().Where(x => x is ConstantNode32bit).ToList();

            foreach (var node in descendantConstants)
            {
                var constNode = node as ConstantNode32bit;
                if (constNode != null)
                {
                    if (!_constantValue.Any(x => x.Item1 == constNode.Value))
                    {
                        _constantValue.Add(new Tuple<uint, string>(constNode.Value, $"C{counter++}"));
                    }
                }
            }
            foreach (var node in descendantConstants)
            {
                var constNode = node as ConstantNode32bit;
                if (constNode != null)
                {
                    var item = _constantValue.FirstOrDefault(x => x.Item1 == constNode.Value);
                    if (item != null)
                    {
                        constNode.Name = item.Item2;
                    }
                }
            }
        }

        internal IGeneticAvalancheFunction GetAvalancheFunction()
        {
            return new GeneticAvalancheFunctionFeistel(GetEngine());
        }
    }
}
