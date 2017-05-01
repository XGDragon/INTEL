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
        public ConnectionCollection Connections = new ConnectionCollection();

        public decimal Fitness { get; private set; }
        public Species MemberOf { get; set; }

        public Genome()
        {
            Nodes.Add(0, new OutputNode(0));
            Nodes.Add(1, new InputNode(1, true)); //bias
            for (int i = 0; i < Parameter.MaxInputNodes; i++)
                Nodes.Add(i + 2, new InputNode(i + 2, false));
            //no hidden in initialization

            Connections.Add(Nodes[1], Nodes[0]);
            for (int i = 0; i < Parameter.InputNodes && i < Parameter.MaxInputNodes; i++)
                Connections.Add(Nodes[i + 2], Nodes[0]);
        }

        public class ConnectionCollection
        {
            private List<Connection> _connections = new List<Connection>();

            public Dictionary<Node, List<Connection>> Sources = new Dictionary<Node, List<Connection>>();
            public Dictionary<Node, List<Connection>> Destinations = new Dictionary<Node, List<Connection>>();

            public Connection this[int i] { get { return _connections[i]; } }

            public void Add(Node a, Node b)
            {
                if (Sources.ContainsKey(a))
                    Sources.Add(a, new List<Connection>());
                if (Destinations.ContainsKey(b))
                    Destinations.Add(b, new List<Connection>());

                Connection c = new Connection(a, b);
                _connections.Add(c);
                Sources[a].Add(c);
                Destinations[b].Add(c);
            }

            public int Count { get { return _connections.Count; } }
        }

        public class NodeCollection
        {
            private Dictionary<int, Node> _nodes = new Dictionary<int, Node>();

            public List<Node> Inputs = new List<Node>();
            public List<Node> Hidden = new List<Node>();
            public List<Node> Outputs = new List<Node>();

            public Node this[int i] { get { return _nodes[i]; } }

            public void Add(int i, Node a)
            {
                _nodes.Add(i, a);

                switch (a.NodeType)
                {
                    case Node.Type.Bias:
                        Inputs.Add(a); break;
                    case Node.Type.Input:
                        Inputs.Add(a); break;
                    case Node.Type.Hidden:
                        Hidden.Add(a); break;
                    case Node.Type.Output:
                        Outputs.Add(a); break;
                }
            }

            public int Count { get { return _nodes.Count; } }
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
