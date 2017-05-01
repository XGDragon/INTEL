using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Genome
    {
        public NodeCollection Nodes = new NodeCollection();
        public List<Connection> Connections = new List<Connection>();

        public decimal Fitness { get; private set; }
        public Species MemberOf { get; set; }

        public Genome()
        {
            Nodes.Add(0, new OutputNode(0));
            Nodes.Add(1, new BiasNode(1));
            for (int i = 0; i < Parameter.MaxInputNodes; i++)
                Nodes.Add(i + 2, new InputNode(i + 2, false));
            //no hidden in initialization

            Connections.Add(Nodes[1].Connect(Nodes[0]));
            for (int i = 0; i < Parameter.InputNodes && i < Parameter.MaxInputNodes; i++)
                Connections.Add(Nodes[i + 2].Connect(Nodes[0]));
        }

        public void EvaluateFitness(Problem.FitnessFunction ff)
        {
            Fitness = ff(this);
        }

        public class NodeCollection
        {
            private Dictionary<int, Node> _nodes = new Dictionary<int, Node>();
            private List<Node> __nodes = new List<Node>();

            private BiasNode _bias;
            private List<InputNode> _inputs = new List<InputNode>();
            private List<HiddenNode> _hidden = new List<HiddenNode>();
            private List<OutputNode> _outputs = new List<OutputNode>();

            public Node this[int i] { get { return _nodes[i]; } }

            public void Add(int i, Node a)
            {
                _nodes.Add(i, a);
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

            public void Inputs(decimal[] inputs)
            {
                for (int i = 0; i < _inputs.Count; i++)
                    _inputs[i].SetInput(inputs[i]);
            }

            public void Activate(Problem.ActivationFunction af, params Node.Type[] types)
            {
                for (int i = 0; i < types.Length; i ++)
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

            public int Count { get { return __nodes.Count; } }
        }

        public static bool operator >(Genome a, Genome b) { return a.Fitness > b.Fitness; }
        public static bool operator <(Genome a, Genome b) { return a.Fitness < b.Fitness; }
        
        //public class Comparison
        //{
        //    public enum Group { Matching, Disjoint, Excess }
        //    public enum Type { Stronger, Weaker }

        //    private Dictionary<Type, Dictionary<Group, List<Connection>>> _dict = new Dictionary<Type, Dictionary<Group, List<Connection>>>();

        //    private Genome _stronger { get { return (_a > _b) ? _a : _b; } }
        //    private Genome _weaker { get { return (_a > _b) ? _b : _a; } }
            
        //    private Genome _a;
        //    private Genome _b;

        //    private List<Connection> _matching;
        //    private List<Connection> _disjoint;
        //    private List<Connection> _excess;       

        //    public Comparison(Genome a, Genome b)
        //    {
        //        _a = a;
        //        _b = b;

        //        List<Connection> larger = (a.Connections.Count > b.Connections.Count) ? a.Connections : b.Connections;
        //    }

        //    public List<Connection> Get(Type t, Group g) { return _dict[t][g]; }

            
        //}
    }
}
