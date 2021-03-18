using EnderPi.Framework.Random;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

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
        private string _animatedGif;

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

        public string AnimatedGifString
        {
            get
            {
                if (_animatedGif == null)
                {
                    RenderAnimatedGif();
                }
                return _animatedGif;
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
        public RngSpecies():this(ConstraintMode.None)
        {          
        }

        public RngSpecies(ConstraintMode mode)
        {
            switch (mode)
            {
                case ConstraintMode.None:
                    _stateOneRoot = new IntronNode(new StateOneNode());
                    _stateTwoRoot = new IntronNode(new StateTwoNode());
                    break;
                case ConstraintMode.StateIncremental:
                    _stateOneRoot = new IntronNode(new AdditionNode(new StateOneNode(), new ConstantNode(1)));
                    _stateTwoRoot = new IntronNode(new AdditionNode(new StateTwoNode(), new ConstantNode(1)));
                    break;
                case ConstraintMode.StateLinearCongruential:
                    _stateOneRoot = new IntronNode(new AdditionNode(new MultiplicationNode(new StateOneNode(), new ConstantNode(3935559000370003845)), new ConstantNode(2691343689449507681)));
                    _stateTwoRoot = new IntronNode(new AdditionNode(new MultiplicationNode(new StateTwoNode(), new ConstantNode(3935559000370003845)), new ConstantNode(2691343689449507681))); new IntronNode(new AdditionNode(new ConstantNode(1), new StateTwoNode()));
                    break;
                case ConstraintMode.StateXorShift:
                    TreeNode leftshift = new LeftShiftNode(new StateOneNode(), new ConstantNode(13));
                    TreeNode firstXorNode = new XorNode(new StateOneNode(), leftshift);
                    TreeNode secondXorNode = new XorNode(firstXorNode, new RightShiftNode(firstXorNode, new ConstantNode(7)));
                    TreeNode thirdXorNode = new XorNode(secondXorNode, new LeftShiftNode(secondXorNode, new ConstantNode(17)));
                    TreeNode leftshift2 = new LeftShiftNode(new StateTwoNode(), new ConstantNode(13));
                    TreeNode firstXorNode2 = new XorNode(new StateTwoNode(), leftshift2);
                    TreeNode secondXorNode2 = new XorNode(firstXorNode2, new RightShiftNode(firstXorNode2, new ConstantNode(7)));
                    TreeNode thirdXorNode2 = new XorNode(secondXorNode2, new LeftShiftNode(secondXorNode2, new ConstantNode(17)));
                    _stateOneRoot = new IntronNode(thirdXorNode);
                    _stateTwoRoot = new IntronNode(thirdXorNode2);
                    break;
                case ConstraintMode.StateWeyl:
                    _stateOneRoot = new IntronNode(new AdditionNode(new StateOneNode(), new ConstantNode(2048534558598693729)));
                    _stateTwoRoot = new IntronNode(new AdditionNode(new StateTwoNode(), new ConstantNode(2048534558598693729)));
                    break;
            }

            _outputRoot = new IntronNode(new AdditionNode(new StateOneNode(), new StateTwoNode()));
            _seedOneRoot = new SeedRootNode(new SeedNode());
            _seedTwoRoot = new SeedRootNode(new SeedNode());
            Birthday = DateTime.Now;
            Name = NameGenerator.GetName();
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
           


        public void RenderAnimatedGif()
        {
            //if (_animatedGif == null)
            //{
            //    MemoryStream ms = new MemoryStream();
            //    var gif = new AnimatedGifCreator(ms, 33);
            //    var mainBitmap = GetImageBitMap();
            //    gif.AddFrame(mainBitmap, 4000);

            //    foreach (var frame in GetVerticalSquishAnimation(mainBitmap))
            //    {
            //        gif.AddFrame(frame);
            //    }
            //    gif.AddFrame(mainBitmap, 4000);
            //    mainBitmap = GetImageBitMap();
            //    gif.AddFrame(mainBitmap, 4000);

            //    foreach (var frame in GetHorizontalSquishAnimation(mainBitmap))
            //    {
            //        gif.AddFrame(frame);
            //    }
            //    gif.AddFrame(mainBitmap, 4000);
            //     AnimatedGif = ms.ToArray();
            //    string base64Data = Convert.ToBase64String(AnimatedGif);
            //    _animatedGif = "data:image/gif;base64," + base64Data;
            //}
        }

        private IEnumerable<Bitmap> GetVerticalSquishAnimation(Bitmap bitmap)
        {            
            bool changeMade;
            do
            {
                changeMade = false;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = bitmap.Height - 1; y > 0 ; y--)
                    {
                        if (bitmap.GetPixel(x,y).ToArgb() == Color.Blue.ToArgb() && bitmap.GetPixel(x, y-1).ToArgb() == Color.Red.ToArgb())
                        {
                            bitmap.SetPixel(x, y, Color.Red);
                            bitmap.SetPixel(x, y-1, Color.Blue);
                            changeMade = true;
                        }                        
                    }                    
                }
                yield return bitmap;
            } while (changeMade);
        }

        private IEnumerable<Bitmap> GetHorizontalSquishAnimation(Bitmap bitmap)
        {
            bool changeMade;
            do
            {
                changeMade = false;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = bitmap.Width - 1; x > 0; x--)
                    {
                        if (bitmap.GetPixel(x, y).ToArgb() == Color.Blue.ToArgb() && bitmap.GetPixel(x-1, y).ToArgb() == Color.Red.ToArgb())
                        {
                            bitmap.SetPixel(x, y, Color.Red);
                            bitmap.SetPixel(x-1, y, Color.Blue);
                            changeMade = true;
                        }
                    }
                }
                yield return bitmap;
            } while (changeMade);
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

        public string GetImageStringSquished()
        {
            string base64Data = string.Empty;
            var bitmap = GetImageSquishedBitmap();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Gif);
            base64Data = Convert.ToBase64String(ms.ToArray());
            return "data:image/gif;base64," + base64Data;
        }

        public byte[] AnimatedGif;

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
                return 2*_stateOneRoot.GetTotalCost() + 2*_stateTwoRoot.GetTotalCost() + _outputRoot.GetTotalCost() + _seedOneRoot.GetTotalCost() + _seedTwoRoot.GetTotalCost();
            }
        }

        /// <summary>
        /// Get an Engine from the generator.  Need to parse the tree to make a string, then use FLEE to get a function, then return the resultant engine?
        /// </summary>
        /// <returns></returns>
        public IEngine GetEngine()
        {
            return new GeneticEngine(_stateOneRoot.Evaluate(), _stateTwoRoot.Evaluate(), _outputRoot.Evaluate(), _seedOneRoot.Evaluate(), _seedTwoRoot.Evaluate());            
        }

        public bool Validate()
        {
            //put validation in here to prevent essentially non-starter RNGs
            throw new NotImplementedException();
        }

        public void NameConstants()
        {
            int counter = 1;
            for (int i=1; i <5; i++)
            {
                var tree = GetTreeRoot(i);
                var descendantConstants = tree.GetDescendants().Distinct().Where(x => x is ConstantNode).ToList();
                List<Tuple<ulong, string>> names = new List<Tuple<ulong, string>>();
                
                foreach (var node in descendantConstants)
                {
                    var constNode = node as ConstantNode;
                    if (constNode != null)
                    {
                        if (!names.Any(x => x.Item1 == constNode.Value))
                        {
                            names.Add(new Tuple<ulong, string>(constNode.Value, $"C{counter++}"));
                        }
                    }                    
                }
                foreach (var node in descendantConstants)
                {
                    var constNode = node as ConstantNode;
                    if (constNode != null)
                    {
                        var item = names.FirstOrDefault(x=>x.Item1 == constNode.Value);
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
