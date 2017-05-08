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
        private Dictionary<int, Node> _nodesDict = new Dictionary<int, Node>();
        private List<Node> _nodesList = new List<Node>();

        private Node _bias;
        private List<Node> _inputs = new List<Node>();
        private List<Node> _hidden = new List<Node>();
        private List<Node> _outputs = new List<Node>();

        private Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();
        public IReadOnlyDictionary<int, Connection> Connections { get { return _connections; } }

        public Node this[int i] { get { return _nodesDict[i]; } }

        public NodeCollection() { }
        
        /// <summary>
        /// Mutation
        /// </summary>
        public NodeCollection(Genome copy) 
        {
            for (int i = 0; i < copy.Nodes.Count; i++)
                Add(new Node(copy.Nodes._nodesList[i]));

            foreach (Connection c in copy.Nodes._connections.Values)
                Connect(c);

            RemoveUselessHiddenNodes();
        }

        /// <summary>
        /// Crossover
        /// </summary>
        public NodeCollection(Genome parent1, Genome parent2)
        {
            for (int i = 0; i < parent1.Nodes.Count; i++)
                Add(new Node(parent1.Nodes._nodesList[i]));
            for (int i = 0; i < parent2.Nodes.Count; i++)
                Add(new Node(parent2.Nodes._nodesList[i]));
            
            GenomeComparison gc = new GenomeComparison(parent1, parent2);
            List<Connection>[] cc = new List<Connection>[4]
            {
                gc[GenomeComparison.Stronger, GenomeComparison.Matching],
                gc[GenomeComparison.Weaker, GenomeComparison.Matching],
                gc[GenomeComparison.Stronger, GenomeComparison.Disjoint],
                gc[GenomeComparison.Stronger, GenomeComparison.Excess]
            };

            for (int i = 1; i < cc.Length; i++)
                for (int j = 0; j < cc[i].Count; j++)
                    if (i == 1) //if 1, then its about matching connections, otherwise..
                        Connect(cc[i - 1][j], cc[i][j]);
                    else
                        Connect(cc[i][j]);

            RemoveUselessHiddenNodes();
        }

        public void Connect(Node a, Node b)
        {
            Connection c = new Connection(a, b);
            _connections.Add(c.Innovation, c);
            a.AddConnection(c);
        }

        private void Connect(Connection copy, Connection copy2 = null)
        {
            Node a = _nodesDict[copy.From.ID];
            Connection c;
            if (copy2 == null)
                c = new Connection(a, _nodesDict[copy.To.ID], copy);
            else
                c = new Connection(a, _nodesDict[copy.To.ID], copy, copy2);
            _connections.Add(c.Innovation, c);
            a.AddConnection(c);
        }

        public void Mutate()
        {
            foreach (Connection c in _connections.Values)
            {
                c.ReEnable();
                c.MutateWeight();
            }

            bool newConnection = Program.R.NextDecimal() < Parameter.MutationAddConnection;
            bool newNode = Program.R.NextDecimal() < Parameter.MutationAddNode;

            if (!newNode && newConnection)
                MutateNewConnection();  //we dont add connections when adding nodes..
            if (newNode)
                MutateNewNode();
        }

        private void MutateNewConnection()
        {
            bool recurrentAllowed = Program.R.NextDecimal() < Parameter.MutationRecurrency;
            List<(Node a, Node b)> potentialConnections = new List<(Node a, Node b)>();
            List<Node>[] ho = new List<Node>[] { _hidden, _outputs };
            
            foreach (Node a in _nodesList)
                foreach (List<Node> l in ho)
                    foreach (Node b in l)
                        if (!a.ConnectsTo(b))
                            if (!(a.IsRecurrent(b) && !recurrentAllowed)) //00=1,01=1,10=0,11=1
                                potentialConnections.Add((a, b));
            //all nodes are eligible as 'from'
            //only hidden and outputs and eligible as 'to'
            var ab = potentialConnections[Program.R.Next(potentialConnections.Count)];
            Connect(ab.a, ab.b);
        }

        private void MutateNewNode()
        {
            int id = _nodesDict.Keys.Max() + 1;
            Node n = new Node(id, Node.Type.Hidden);
            Add(n);

            //all node to nodes are eligible where the connection is enabled
            var eligible = _connections.Values.Where(c => { return c.Enable; });
            int count = eligible.Count();
            if (count > 0)
            {
                Connection cut = eligible.ElementAt(Program.R.Next(count));
                Connect(cut.From, n);
                Connect(n, cut.To);
                cut.Enable = false;
            }
        }

        private void RemoveUselessHiddenNodes()
        {
            _hidden.RemoveAll(h =>
            {
                return _nodesList.TrueForAll(n => { return !n.ConnectsTo(h); });
            });
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
            decimal[] d = new decimal[_nodesList.Count];
            for (int i = 0; i < _nodesList.Count; i++)
                d[i] = _nodesList[i].Output;
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
            if (_nodesDict.ContainsKey(a.ID))
                return;

            _nodesDict.Add(a.ID, a);
            _nodesList.Add(a);

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
            _nodesDict.Clear();
            _nodesList.Clear();

            _bias = null;
            _inputs.Clear();
            _hidden.Clear();
            _outputs.Clear();
        }

        public bool Contains(Node item)
        {
            return _nodesList.Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex)
        {
            _nodesList.CopyTo(array, arrayIndex);
        }

        public bool Remove(Node a)
        {
            _nodesDict.Remove(a.ID);

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

            return _nodesList.Remove(a);
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return _nodesList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodesList.GetEnumerator();
        }

        public int Count { get { return _nodesList.Count; } }

        public bool IsReadOnly => ((ICollection<Node>)_nodesList).IsReadOnly;
        #endregion
    }
}
