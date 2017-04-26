using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Genome
    {
        public Dictionary<int, Node> Nodes = new Dictionary<int, Node>();
        public List<Connection> Connections = new List<Connection>();

        public decimal Fitness { get; private set; }
        public Species MemberOf { get; private set; }

        public Genome()
        {
            Nodes.Add(0, new OutputNode(0));
            Nodes.Add(1, new InputNode(1, true)); //bias
            for (int i = 0; i < Parameter.MaxInputNodes; i++)
                Nodes.Add(i + 2, new InputNode(i + 2, false));
            //no hidden in initialization

            Connections.Add(new Connection(Nodes[1], Nodes[0]));
            for (int i = 0; i < Parameter.InputNodes && i < Parameter.MaxInputNodes; i++)
                Connections.Add(new Connection(Nodes[i + 2], Nodes[0]));
        }

        public void ChangeSpecies(Species s)
        {
            MemberOf = s;
            s.Add(this);
        }

        public (Connection[] matching, Connection[] disjoint, Connection[] excess)[] CompareWith(Genome b)
        {
            Genome a = this;
            (Connection[] matching, Connection[] disjoint, Connection[] excess)[] r = new(Connection[] matching, Connection[] disjoint, Connection[] excess)[2];

            int i = 0;
            for (; i < a.Connections.Count; i++)
            {

            }
        }
    }
}
