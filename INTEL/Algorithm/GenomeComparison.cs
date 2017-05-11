using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL.Network
{
    public class GenomeComparison
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
        private double _w = 0;
        private int _n = 0;

        public double Distance { get; private set; }

        public GenomeComparison(Genome a, Genome b)
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 3; j++)
                    _connections[i, j] = new List<Connection>();

            if (a == b)
            {
                foreach (Connection c in a.Nodes.Connections.Values)
                {
                    Add(Stronger, Matching, c);
                    Add(Weaker, Matching, c);
                }
                return;
            }

            _n = Math.Max(a.Nodes.Connections.Count, b.Nodes.Connections.Count);
            if (_n < 20)
                _n = 1;

            _stronger = (a > b) ? a : b;
            _weaker = (a > b) ? b : a;
            var s = _stronger.Nodes.Connections;
            var w = _weaker.Nodes.Connections;

            (int a, int b) t = (s.Max(n => n.Key), w.Max(n => n.Key));
            (int top, int border) x = (t.a > t.b) ? (t.a, t.b) : (t.b, t.a);
            for (int i = 0; i <= x.top; i++)
            {
                if (s.ContainsKey(i) && w.ContainsKey(i))
                {
                    Add(Stronger, Matching, s[i]);
                    Add(Weaker, Matching, w[i]);
                    _w += Math.Abs(s[i].Weight - w[i].Weight);
                }
                else
                {
                    uint group = (i > x.border) ? Excess : Disjoint;
                    if (s.ContainsKey(i))
                        Add(Stronger, group, s[i]);
                    if (w.ContainsKey(i))
                        Add(Weaker, group, w[i]);
                }
            }

            double e = (Parameter.c1 * _excess) / _n;
            double d = (Parameter.c2 * _disjoint) / _n;
            Distance = e + d + (Parameter.c3 * _w);

            //Dictionary<int, Genome> g1 = new Dictionary<int, Genome>();
            //Dictionary<int, Genome> g2 = new Dictionary<int, Genome>();
            //int g1m = -1; int g2m = -1;

            //foreach (int key in s.Keys)
            //{
            //    g1[key] = _stronger;
            //    g1m = (key > g1m) ? key : g1m;
            //}
            //foreach (int key in w.Keys)
            //{
            //    g2[key] = _weaker;
            //    g2m = (key > g2m) ? key : g2m;
            //}
            //int top = (g1m > g2m) ? g1m : g2m;
            //int border = (g1m > g2m) ? g2m : g1m;

            //for (int i = 0; i <= g1m || i <= g2m; i++)
            //{
            //    if (g1.ContainsKey(i) && g2.ContainsKey(i))
            //    {

            //    }
            //}

            //var x = _stronger.Nodes.Connections.Keys.Union(_weaker.Nodes.Connections.Keys).OrderByDescending(i => i);

            //var sb = x.TakeWhile(i => s.ContainsKey(i) && !w.ContainsKey(i));
            //var wb = x.TakeWhile(i => !s.ContainsKey(i) && w.ContainsKey(i));

            //int border = (sb.Any()) ? sb.Last() : ((wb.Any()) ? wb.Last() : int.MaxValue);
            //foreach (var i in x)
            //{
            //    if (s.ContainsKey(i) && w.ContainsKey(i))
            //    {
            //        Add(Stronger, Matching, s[i]);
            //        Add(Weaker, Matching, w[i]);
            //        _w += Math.Abs(s[i].Weight - w[i].Weight);
            //    }
            //    else
            //    {
            //        uint group = (i >= border) ? Excess : Disjoint;
            //        if (s.ContainsKey(i))
            //            Add(Stronger, group, s[i]);
            //        if (w.ContainsKey(i))
            //            Add(Weaker, group, w[i]);
            //    }
            //}

            //double e = (Parameter.c1 * _excess) / _n;
            //double d = (Parameter.c2 * _disjoint) / _n;
            //Distance = e + d + (Parameter.c3 * _w);
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
