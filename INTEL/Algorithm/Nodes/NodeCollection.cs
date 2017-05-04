using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class NodeCollection : ICollection<Node>
    {
        private Dictionary<int, Node> _nodes = new Dictionary<int, Node>();
        private List<Node> __nodes = new List<Node>();

        private Node _bias;
        private List<Node> _inputs = new List<Node>();
        private List<Node> _hidden = new List<Node>();
        private List<Node> _outputs = new List<Node>();

        public Node this[int i] { get { return _nodes[i]; } }

        public NodeCollection() { }
        
        public NodeCollection(NodeCollection a)
        {
            for (int i = 0; i < a.Count; i++)
                Add(new Node(a.__nodes[i]));
        }

        public NodeCollection(NodeCollection a, NodeCollection b) : this(a)
        {            
            for (int i = 0; i < b.Count; i++)
                Add(new Node(b.__nodes[i]));
        }

        /// <summary>
        /// Obtain inputs for Input Nodes
        /// </summary>
        /// <param name="inputs">Array of inputs, where InputNode[0].Input is set to inputs[0]</param>
        public void Accept(decimal[] inputs)
        {
            for (int i = 0; i < _inputs.Count && i < inputs.Length; i++)
                _inputs[i].Input = inputs[i];
        }

        /// <summary>
        /// Activate groups of nodes, transforming input into output using the specified Activation Function. Inputs are set to 0 after activation.
        /// </summary>
        /// <param name="af">Problem-specific Activation Function</param>
        /// <param name="types">Node groups</param>
        public void Activate(Problem.ActivationFunction af, params Node.Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                switch (types[i])
                {
                    case Node.Type.Bias:
                        _bias.Activation(af); break;
                    case Node.Type.Hidden:
                        _hidden.ForEach((Node n) => { n.Activation(af); }); break;
                    case Node.Type.Output:
                        _outputs.ForEach((Node n) => { n.Activation(af); }); break;
                    case Node.Type.Input:
                        _inputs.ForEach((Node n) => { n.Activation(af); }); break;
                }
            }
        }

        /// <summary>
        /// Reports a vector of all node Outputs.
        /// </summary>
        public decimal[] AllOutputs()
        {
            decimal[] d = new decimal[__nodes.Count];
            for (int i = 0; i < __nodes.Count; i++)
                d[i] = __nodes[i].Output;
            return d;
        }

        /// <summary>
        /// Reports a vector of all Output node Outputs.
        /// </summary>
        public decimal[] Outputs()
        {
            decimal[] d = new decimal[_outputs.Count];
            for (int i = 0; i < _outputs.Count; i++)
                d[i] = _outputs[i].Output;
            return d;
        }

        #region ICollection implementation
        public void Add(Node a)
        {
            if (_nodes.ContainsKey(a.ID))
                return;

            _nodes.Add(a.ID, a);
            __nodes.Add(a);

            switch (a.NodeType)
            {
                case Node.Type.Bias:
                    _bias = a; break;
                case Node.Type.Input:
                    _inputs.Add(a); break;
                case Node.Type.Hidden:
                    _hidden.Add(a); break;
                case Node.Type.Output:
                    _outputs.Add(a); break;
            }
        }

        public void Clear()
        {
            _nodes.Clear();
            __nodes.Clear();

            _bias = null;
            _inputs.Clear();
            _hidden.Clear();
            _outputs.Clear();
        }

        public bool Contains(Node item)
        {
            return __nodes.Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex)
        {
            __nodes.CopyTo(array, arrayIndex);
        }

        public bool Remove(Node a)
        {
            _nodes.Remove(a.ID);

            switch (a.NodeType)
            {
                case Node.Type.Bias:
                    _bias = null; break;
                case Node.Type.Input:
                    _inputs.Remove(a); ; break;
                case Node.Type.Hidden:
                    _hidden.Remove(a); break;
                case Node.Type.Output:
                    _outputs.Remove(a); break;
            }

            return __nodes.Remove(a);
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return __nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return __nodes.GetEnumerator();
        }

        public int Count { get { return __nodes.Count; } }

        public bool IsReadOnly => ((ICollection<Node>)__nodes).IsReadOnly;
        #endregion
    }
}
