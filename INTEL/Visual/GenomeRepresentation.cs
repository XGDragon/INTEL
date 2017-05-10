using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INTEL.Visual
{
    public partial class GenomeRepresentation : UserControl
    {
        private INTEL.Network.Genome _genome;

        private Dictionary<INTEL.Network.Node, Node> _nodes = new Dictionary<Network.Node, Node>();        
        private List<(Node a, Node b)> _connections = new List<(Node a, Node b)>();

        private int _nodeSize = 50;

        public GenomeRepresentation(INTEL.Network.Genome g)
        {
            InitializeComponent();
            _genome = g;

            var nodes = _genome.Nodes.List;
            for (int i = 0; i < nodes.Count; i++)
                _nodes.Add(nodes[i], new Node(nodes[i]));
            
            var connections = _genome.Nodes.Connections.ToArray();
            for (int i = 0; i < connections.Length; i++)
                _connections.Add((_nodes[connections[i].Value.From], _nodes[connections[i].Value.To]));

            Paint += GenomeRepresentation_Paint;
            Invalidate();
        }

        private void GenomeRepresentation_Paint(object sender, PaintEventArgs e)
        {
            Dictionary<double, List<Node>> _n = new Dictionary<double, List<Node>>();
            foreach (Node n in _nodes.Values)
            {
                if (!_n.ContainsKey(n.Depth))
                    _n.Add(n.Depth, new List<Node>());
                _n[n.Depth].Add(n);
            }

            foreach (double d in _n.Keys)
            {
                int n = _n[d].Count;
                _n[d].Sort();
                for (int i = 0; i < n; i++)
                    PaintNode(e.Graphics, _n[d][i], new Point(IDToX(i, n), DepthToY(_n[d][i].Depth)));
            }
        }

        private int DepthToY(double depth)
        {         
            var maxH = Height - _nodeSize;
            return maxH - (int)Math.Round(depth * maxH);
        }

        private int IDToX(int i, int n)
        {
            int maxX = Width - _nodeSize;
            return ((maxX / n) * i) + (maxX / n) / 2;
        }

        private void PaintNode(Graphics g, Node n, Point topleft)
        {
            g.DrawEllipse(Pens.SlateGray, new Rectangle(topleft, new Size(_nodeSize, _nodeSize)));

        }

        private struct Node : IComparable<Node>
        {
            public double Depth { get { return _node.Depth; } }
            public int ID { get { return _node.ID; } }

            private INTEL.Network.Node _node;

            public Node(INTEL.Network.Node node)
            {
                _node = node;
            }   

            public override string ToString()
            {
                return ID + " @ " + Depth;
            }

            public int CompareTo(Node other)
            {
                
            }
        }
    }
}
