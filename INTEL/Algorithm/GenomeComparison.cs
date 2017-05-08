using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class GenomeComparison
    {
        public const uint Matching = 0;
        public const uint Disjoint = 1;
        public const uint Excess = 2;

        public const uint Stronger = 0;
        public const uint Weaker = 1;

        private Genome _stronger;
        private Genome _weaker;
        
        private List<Connection>[,] _connections = new List<Connection>[2, 3];

        private int _excess = 0;
        private int _disjoint = 0;
        private decimal _w = 0;
        private int _n = 0;

        public decimal Distance { get; private set; }

        public GenomeComparison(Genome a, Genome b)
        {
            _n = Math.Max(a.Nodes.Connections.Count, b.Nodes.Connections.Count);

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 3; j++)
                    _connections[i, j] = new List<Connection>();

            _stronger = (a > b) ? a : b;
            _weaker = (a > b) ? b : a;
            var s = _stronger.Nodes.Connections;
            var w = _weaker.Nodes.Connections;

            var x = _stronger.Nodes.Connections.Keys.Union(_weaker.Nodes.Connections.Keys).OrderByDescending(i => i);

            var sb = x.TakeWhile(i => s.ContainsKey(i) && !w.ContainsKey(i));
            var wb = x.TakeWhile(i => !s.ContainsKey(i) && w.ContainsKey(i));

            int border = (sb.Any()) ? sb.Last() : ((wb.Any()) ? wb.Last() : int.MaxValue);
            foreach (var i in x)
            {
                if (s.ContainsKey(i) && w.ContainsKey(i))
                {
                    Add(Stronger, Matching, s[i]);
                    Add(Weaker, Matching, w[i]);
                    _w += Math.Abs(s[i].Weight - w[i].Weight);
                }
                else
                {
                    uint group = (i >= border) ? Excess : Disjoint;
                    if (s.ContainsKey(i))
                        Add(Stronger, group, s[i]);
                    if (w.ContainsKey(i))
                        Add(Weaker, group, w[i]);
                }
            }

            decimal e = (Parameter.c1 * _excess) / _n;
            decimal d = (Parameter.c2 * _disjoint) / _n;
            Distance = e + d + (Parameter.c3 * _w);
        }

        private void Add(uint type, uint group, Connection c)
        {
            _connections[type, group].Add(c);
            switch (group)
            {
                case Disjoint:
                    _disjoint++; break;
                case Excess:
                    _excess++; break;
            }
        }

        public List<Connection> this[uint i, uint j] { get { return _connections[i, j]; } }
    }
}
