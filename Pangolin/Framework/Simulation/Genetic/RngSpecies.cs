using EnderPi.Framework.Random;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace EnderPi.Framework.Simulation.Genetic
{
    [Serializable]
    public class RngSpecies
    {
        private TreeNode _stateOneRoot;
        private TreeNode _stateTwoRoot;
        private TreeNode _outputRoot;
        private TreeNode _seedOneRoot;
        private TreeNode _seedTwoRoot;
        private string _imageString;
        private string _imageStringSquished;
        private bool _useStateTwo;

        /// <summary>
        /// How fit this specimen is.  Higher values are better.
        /// </summary>
        public double Fitness { set; get; }

        /// <summary>
        /// The generation this species was created during.
        /// </summary>
        public int Generation { set; get; }

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
        private List<Tuple<ulong, string>> _constantValue;

        public List<Tuple<ulong, string>> ConstantNameList { get { return _constantValue; } }

        public string ConstantNames
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (_constantValue != null)
                {
                    foreach(var val in _constantValue)
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

        public string ImageStringSquished
        {
            get
            {
                if (_imageStringSquished == null)
                {
                    _imageStringSquished = GetImageStringSquished();
                }
                return _imageStringSquished;
            }
        }
        

        public TreeNode GetTreeRoot(int index)
        {
            switch (index)
            {
                case 1:
                    return _stateOneRoot;
                case 2:
                    return _stateTwoRoot;
                case 3:
                    return _outputRoot;
                case 4:
                    return _seedOneRoot;
                case 5:
                    return _seedTwoRoot;
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        /// <summary>
        /// Default constructor, generates very boring species with zero state transition and just the addition of the two states as output.
        /// </summary>
        public RngSpecies():this(ConstraintMode.None, ConstraintMode.None, true)
        {          
        }

        public RngSpecies(ConstraintMode modeOne, ConstraintMode modeTwo, bool useStateTwo)
        {
            _useStateTwo = useStateTwo;
            switch (modeOne)
            {
                case ConstraintMode.None:
                    if (useStateTwo)
                    {
                        _stateOneRoot = new IntronNode(new AdditionNode(new StateOneNode(), new StateTwoNode()));
                    }
                    else
                    {
                        _stateOneRoot = new IntronNode(new StateOneNode());
                    }
                    break;
                case ConstraintMode.StateInc:
                    _stateOneRoot = new IntronNode(new AdditionNode(new StateOneNode(), new ConstantNode(1)));
                    break;
                case ConstraintMode.StateLcg:
                    _stateOneRoot = new IntronNode(new AdditionNode(new MultiplicationNode(new StateOneNode(), new ConstantNode(3935559000370003845)), new ConstantNode(2691343689449507681)));
                    break;
                case ConstraintMode.StateXor:
                    TreeNode leftshift = new LeftShiftNode(new StateOneNode(), new ConstantNode(13));
                    TreeNode firstXorNode = new XorNode(new StateOneNode(), leftshift);
                    TreeNode secondXorNode = new XorNode(firstXorNode, new RightShiftNode(firstXorNode, new ConstantNode(7)));
                    TreeNode thirdXorNode = new XorNode(secondXorNode, new LeftShiftNode(secondXorNode, new ConstantNode(17)));
                    _stateOneRoot = new IntronNode(thirdXorNode);
                    break;
                case ConstraintMode.StateWeyl:
                    _stateOneRoot = new IntronNode(new AdditionNode(new StateOneNode(), new ConstantNode(2048534558598693729)));
                    break;
            }
            switch (modeTwo)
            {
                case ConstraintMode.None:
                    _stateTwoRoot = new IntronNode(new AdditionNode(new StateOneNode(), new StateTwoNode()));
                    break;
                case ConstraintMode.StateInc:
                    _stateTwoRoot = new IntronNode(new AdditionNode(new StateTwoNode(), new ConstantNode(1)));
                    break;
                case ConstraintMode.StateLcg:
                    _stateTwoRoot = new IntronNode(new AdditionNode(new MultiplicationNode(new StateTwoNode(), new ConstantNode(3935559000370003845)), new ConstantNode(2691343689449507681))); new IntronNode(new AdditionNode(new ConstantNode(1), new StateTwoNode()));
                    break;
                case ConstraintMode.StateXor:
                    TreeNode leftshift2 = new LeftShiftNode(new StateTwoNode(), new ConstantNode(13));
                    TreeNode firstXorNode2 = new XorNode(new StateTwoNode(), leftshift2);
                    TreeNode secondXorNode2 = new XorNode(firstXorNode2, new RightShiftNode(firstXorNode2, new ConstantNode(7)));
                    TreeNode thirdXorNode2 = new XorNode(secondXorNode2, new LeftShiftNode(secondXorNode2, new ConstantNode(17)));
                    _stateTwoRoot = new IntronNode(thirdXorNode2);
                    break;
                case ConstraintMode.StateWeyl:
                    _stateTwoRoot = new IntronNode(new AdditionNode(new StateTwoNode(), new ConstantNode(2048534558598693729)));
                    break;
            }

            if (useStateTwo)
            {
                _outputRoot = new IntronNode(new AdditionNode(new StateOneNode(), new StateTwoNode()));
            }
            else
            {
                _outputRoot = new IntronNode(new StateOneNode());
            }
            _seedOneRoot = new SeedRootNode(new SeedNode());
            _seedTwoRoot = new SeedRootNode(new SeedNode());
            Birthday = DateTime.Now;            
        }

        public void SetNodeAge(int age)
        {
            for (int i = 1; i <= 5; i++)
            {
                var root = GetTreeRoot(i);
                root.GenerationOfOrigin = age;
                foreach (var descendant in root.GetDescendants())
                {
                    descendant.GenerationOfOrigin = age;
                }
            }
        }

        public double GetAverageNodeAge()
        {
            double average = 0;
            int count = 0;
            for (int i = 1; i <= 5; i++)
            {
                var root = GetTreeRoot(i);
                foreach (var descendant in root.GetDescendants())
                {
                    average += descendant.GenerationOfOrigin;
                    count++;
                }
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

        public RngSpecies(TreeNode stateOne, TreeNode stateTwo, TreeNode output, TreeNode seedOne, TreeNode seedTwo)
        {
            _stateOneRoot = new IntronNode(stateOne);
            _stateTwoRoot = new IntronNode(stateTwo);
            _outputRoot = new IntronNode(output);
            _seedOneRoot =new SeedRootNode( seedOne);
            _seedTwoRoot = new SeedRootNode(seedTwo);
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

        private Bitmap GetImageSquishedBitmap()
        {
            var engine = GetEngine() as Engine;
            engine.Seed(Seed);
            var bitmap = engine.GetBitMap(4096);
            for (int x = 0; x < bitmap.Width; x++)
            {
                //ok, so this loops over one direction
                int count = 0;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.ToArgb() == Color.Red.ToArgb())
                    {
                        bitmap.SetPixel(x, y, Color.Blue);
                        count++;
                    }
                }
                for (int y = 0; y < count; y++)
                {
                    bitmap.SetPixel(x, y, Color.Red);
                }
            }
            return bitmap;
        }

        internal bool IsValid()
        {
            bool stateOneHasStateOne = _stateOneRoot.GetDescendants().Any(x => x is StateOneNode);
            bool stateOneHasStateTwo = _stateOneRoot.GetDescendants().Any(x => x is StateTwoNode);
            bool stateTwoHasStateOne = _stateTwoRoot.GetDescendants().Any(x => x is StateOneNode);
            bool stateTwoHasStateTwo = _stateTwoRoot.GetDescendants().Any(x => x is StateTwoNode);
            bool outputHasStateOne = _outputRoot.GetDescendants().Any(x => x is StateOneNode);
            bool outputHasStateTwo = _outputRoot.GetDescendants().Any(x => x is StateTwoNode);
            if (_useStateTwo)
            {
                if (!outputHasStateOne || !outputHasStateTwo)
                {
                    return false;
                }                
                //If it doesn't have state ONE and it doesn't have state two, that's bad.
                if (!stateOneHasStateOne && !stateOneHasStateTwo)
                {
                    return false;
                }
                if (!stateTwoHasStateOne && !stateTwoHasStateTwo)
                {
                    return false;
                }
                //todo seed validation.
            }
            else
            {
                if (!outputHasStateOne || outputHasStateTwo)
                {
                    return false;
                }
                if (!stateOneHasStateOne || stateOneHasStateTwo)
                {
                    return false;
                }
            }
            return true;
        }

        public string GetImageStringSquished()
        {
            string base64Data = string.Empty;
            var bitmap = GetImageSquishedBitmap();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Gif);
            base64Data = Convert.ToBase64String(ms.ToArray());
            return "data:image/gif;base64," + base64Data;
        }                

        /// <summary>
        /// Gets the total node count from all three trees.
        /// </summary>
        public int NodeCount
        {
            get
            {
                return _stateOneRoot.GetTotalNodeCount() + _stateTwoRoot.GetTotalNodeCount() + _outputRoot.GetTotalNodeCount() + _seedOneRoot.GetTotalNodeCount() + _seedTwoRoot.GetTotalNodeCount();
            }
        }

        public double TotalCost
        {
            get
            {
                return _stateOneRoot.GetTotalCost() + _stateTwoRoot.GetTotalCost() + _outputRoot.GetTotalCost() + _seedOneRoot.GetTotalCost() + _seedTwoRoot.GetTotalCost();
            }
        }

        /// <summary>
        /// Get an Engine from the generator.  Need to parse the tree to make a string, then use FLEE to get a function, then return the resultant engine?
        /// </summary>
        /// <returns></returns>
        public IEngine GetEngine()
        {
            if (!_useStateTwo)
            {
                return new GeneticEngineOneState(_stateOneRoot.Evaluate(), _outputRoot.Evaluate(), _seedOneRoot.Evaluate());
            }
            else
            {
                return new GeneticEngine(_stateOneRoot.Evaluate(), _stateTwoRoot.Evaluate(), _outputRoot.Evaluate(), _seedOneRoot.Evaluate(), _seedTwoRoot.Evaluate());
            }
        }                

        /// <summary>
        /// Names all the constants so it can be displayed appropriately.
        /// </summary>
        public void NameConstants()
        {
            int counter = 1;
            _constantValue = new List<Tuple<ulong, string>>();
            for (int i=1; i <5; i++)
            {
                var tree = GetTreeRoot(i);
                var descendantConstants = tree.GetDescendants().Distinct().Where(x => x is ConstantNode).ToList();
                                
                foreach (var node in descendantConstants)
                {
                    var constNode = node as ConstantNode;
                    if (constNode != null)
                    {
                        if (!_constantValue.Any(x => x.Item1 == constNode.Value))
                        {
                            _constantValue.Add(new Tuple<ulong, string>(constNode.Value, $"C{counter++}"));
                        }
                    }                    
                }
                foreach (var node in descendantConstants)
                {
                    var constNode = node as ConstantNode;
                    if (constNode != null)
                    {
                        var item = _constantValue.FirstOrDefault(x=>x.Item1 == constNode.Value);
                        if (item != null)
                        {
                            constNode.Name = item.Item2;
                        }
                    }
                }
            }
        }
                

    }
}
