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

        private BiasNode _bias;
        private List<InputNode> _inputs = new List<InputNode>();
        private List<HiddenNode> _hidden = new List<HiddenNode>();
        private List<OutputNode> _outputs = new List<OutputNode>();

        public Node this[int i] { get { return _nodes[i]; } }

        public void Accept(decimal[] inputs)
        {
            for (int i = 0; i < _inputs.Count; i++)
                _inputs[i].SetInput(inputs[i]);
        }

        public void Activate(Problem.ActivationFunction af, params Node.Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                switch (types[i])
                {
                    case Node.Type.Bias:
                        _bias.Activation(af); break;
                    case Node.Type.Hidden:
                        _hidden.ForEach((HiddenNode n) => { n.Activation(af); }); break;
                    case Node.Type.Output:
                        _outputs.ForEach((OutputNode n) => { n.Activation(af); }); break;
                    case Node.Type.Input:
                        _inputs.ForEach((InputNode n) => { n.Activation(af); }); break;
                }
            }
        }

        public decimal[] Outputs()
        {
            decimal[] d = new decimal[__nodes.Count];
            for (int i = 0; i < __nodes.Count; i++)
                d[i] = __nodes[i].Output;
            return d;
        }

        public void Add(Node a)
        {
            _nodes.Add(a.ID, a);
            __nodes.Add(a);

            switch (a.NodeType)
            {
                case Node.Type.Bias:
                    _bias = a as BiasNode; break;
                case Node.Type.Input:
                    _inputs.Add(a as InputNode); break;
                case Node.Type.Hidden:
                    _hidden.Add(a as HiddenNode); break;
                case Node.Type.Output:
                    _outputs.Add(a as OutputNode); break;
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
                    _inputs.Remove(a as InputNode); ; break;
                case Node.Type.Hidden:
                    _hidden.Remove(a as HiddenNode); break;
                case Node.Type.Output:
                    _outputs.Remove(a as OutputNode); break;
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
    }
}
