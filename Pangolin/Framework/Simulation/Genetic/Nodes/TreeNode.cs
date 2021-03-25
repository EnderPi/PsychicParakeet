using System;
using System.Collections.Generic;
using System.Linq;
using Flee.PublicTypes;

namespace EnderPi.Framework.Simulation.Genetic
{
    /// <summary>
    /// Tree operator, for chromosomal representation of RNG.
    /// </summary>
    [Serializable]
    public abstract class TreeNode
    {
        /// <summary>
        /// Return the string representation of this node.
        /// </summary>
        /// <returns></returns>
        public abstract string Evaluate();

        protected List<TreeNode> _children = new List<TreeNode>();

        public bool IsLeafNode { get { return _children == null || _children.Count == 0; } }

        public abstract double Cost();

        /// <summary>
        /// When this node was created.  For tracking how things work out.
        /// </summary>
        public int GenerationOfOrigin { set; get; }

        public double GetTotalCost()
        {
            var descendants = GetDescendants();
            descendants.Add(this);
            descendants = descendants.Distinct().ToList();
            return descendants.Sum(x => x.Cost());
        }

        public int GetTotalNodeCount()
        {
            var nodes = GetDescendants();
            return nodes.Distinct().Count() + 1;
        }

        public int GetDescendantsNodeCount()
        {
            var nodes = GetDescendants();
            return nodes.Distinct().Count();
        }

        public List<TreeNode> GetDescendants(int maxDepth = 100)
        {
            if (maxDepth-- < 1)
            {
                throw new Exception("Maximum recursion depth exceeded!");
            }
            var nodes = new List<TreeNode>();
            nodes.AddRange(_children);
            foreach(var node in _children)
            {
                nodes.AddRange(node.GetDescendants(maxDepth));
            }
            return nodes;
        }

        public void ReplaceAllChildReferences(TreeNode nodeToReplace, TreeNode nodeToReplaceWith)
        {
            var children = GetDescendants();
            foreach (var child in children)
            {
                child.ReplaceReferences(nodeToReplace, nodeToReplaceWith);
            }
            ReplaceReferences(nodeToReplace, nodeToReplaceWith);
        }

        private void ReplaceReferences(TreeNode nodeToReplace, TreeNode nodeToReplaceWith)
        {
            if (_children != null)
            {
                for (int i=0; i< _children.Count; i++)
                {
                    if (_children[i] == nodeToReplace)
                    {
                        _children[i] = nodeToReplaceWith;
                    }
                }
            }            
        }

        /// <summary>
        /// Determines whether or not the given child is a tree of this node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsChild(TreeNode node)
        {
            if (_children == null || _children.Count == 0)
            {
                return false;
            }
            if (_children.Contains(node))
            {
                return true;
            }
            return false;
        }

        public bool IsFoldable()
        {
            if (_children != null && _children.Count != 0 && _children.All(x=>x is ConstantNode))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the value of this as a constant.  Probably just throws if misused.
        /// </summary>
        /// <returns></returns>
        public ulong Fold()
        {            
            var context = new ExpressionContext();
            context.Imports.AddType(typeof(Math));
            var expressionStateOne = context.CompileGeneric<ulong>(Evaluate());
            ulong x = expressionStateOne.Evaluate();            
            return x <= long.MaxValue ? x : x ^ (1UL << 63);
        }

        public TreeNode GetFirstChild()
        {
            return _children[0];
        }

        public TreeNode GetSecondChild()
        {
            return _children[1];
        }

        public void ReplaceFirstChild(TreeNode replacement)
        {
            _children[0] = replacement;
        }

        /// <summary>
        /// Returns a more human-readable version.
        /// </summary>
        /// <returns></returns>
        public abstract string EvaluatePretty();

        internal bool IsBinaryNode()
        {
            return (_children != null && _children.Count == 2);
        }
    }
}
