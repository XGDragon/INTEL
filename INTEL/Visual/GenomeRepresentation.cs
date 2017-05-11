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
        
        private List<Node> _nodes = new List<Node>();
        private List<(Node a, Node b, double w, bool e)> _connections = new List<(Node, Node, double, bool)>();

        private int _nodeSize = 40;

        public GenomeRepresentation(INTEL.Network.Genome g)
        {
            InitializeComponent();
            _genome = g;

            //Translate INTEL.Nodes to internal Nodes
            var nodes = _genome.Nodes.List;
            Dictionary<INTEL.Network.Node, Node> trans = new Dictionary<Network.Node, Node>();
            for (int i = 0; i < nodes.Count; i++)
                trans.Add(nodes[i], new Node(nodes[i]));
            
            //Add connections to each internal node
            var connections = _genome.Nodes.Connections.ToArray();
            for (int i = 0; i < connections.Length; i++)
                _connections.Add((trans[connections[i].Value.From], trans[connections[i].Value.To], connections[i].Value.Weight, connections[i].Value.Enable));

            //Split nodes by depth
            Dictionary<double, List<Node>> depth = new Dictionary<double, List<Node>>();
            foreach (Node n in trans.Values)
            {
                if (!depth.ContainsKey(n.Depth))
                    depth.Add(n.Depth, new List<Node>());
                depth[n.Depth].Add(n);
            }

            //Give nodes locations
            foreach (double d in depth.Keys)
            {
                int n = depth[d].Count;
                depth[d].Sort();
                for (int i = 0; i < n; i++)
                {
                    depth[d][i].Location = new Point(IDToX(i, n), DepthToY(depth[d][i].Depth));
                    _nodes.Add(depth[d][i]);
                }
            }

            Paint += GenomeRepresentation_Paint;
            Invalidate();
        }

        private void GenomeRepresentation_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            Font f = new Font(FontFamily.GenericMonospace, 8);
            Pen p = new Pen(Brushes.Black, 1.5f);
            Pen pd = new Pen(Brushes.SlateGray, 1.5f);
            Point br = new Point(_nodeSize / 2, _nodeSize / 2);

            //Paint connections
            foreach ((Node a, Node b, double w, bool e) connection in _connections)
            {
                Point centera = new Point(connection.a.Location.X + br.X, connection.a.Location.Y + br.Y);
                Point centerb = new Point(connection.b.Location.X + br.X, connection.b.Location.Y + br.Y);
                Point weight = new Point((centera.X + centerb.X) / 2, (centera.Y + centerb.Y) / 2);
                Pen u = (connection.e) ? p : pd;
                e.Graphics.DrawLine(u, centera, centerb);
                e.Graphics.DrawString(connection.w.ToString("F"), f, Brushes.Black, weight);
            }
            //Paint nodes
            foreach (Node n in _nodes)
            {
                Rectangle r = new Rectangle(n.Location, new Size(_nodeSize, _nodeSize));
                e.Graphics.FillEllipse(Brushes.WhiteSmoke, r);
                e.Graphics.DrawEllipse(p, r);
                e.Graphics.DrawString(n.ID + "\n" + n.Type, f, Brushes.Black, r, sf);
            }
        }

        private int DepthToY(double depth)
        {         
            var maxH = Height - _nodeSize - Node.RANDOM_OFFSET;
            return maxH - (int)Math.Round(depth * maxH);
        }

        private int IDToX(int i, int n)
        {
            int maxX = Width - _nodeSize;
            return ((maxX / n) * i) + (maxX / n) / 2;
        }
        
        private class Node : IComparable<Node>
        {
            public double Depth { get { return _node.Depth; } }
            public int ID { get { return _node.ID; } }
            public string Type {
                get
                { 
                    switch (_node.NodeType)
                    {
                        case Network.Node.Type.Bias:
                            return "B";
                        case Network.Node.Type.Hidden:
                            return "H";
                        case Network.Node.Type.Input:
                            return "In";
                        case Network.Node.Type.Output:
                        default:
                            return "Out";
                    }
                }
            }

            public const int RANDOM_OFFSET = 30;
            private Point _location;
            public Point Location { get { return _location; } set { _location = new Point(value.X + Program.R.Next(RANDOM_OFFSET), value.Y + Program.R.Next(RANDOM_OFFSET)); }  }

            private INTEL.Network.Node _node;

            public Node(INTEL.Network.Node node)
            {
                _node = node;
            }   

            public override string ToString()
            {
                return ID + " @depth " + Depth + ", " + Location.ToString();
            }

            public int CompareTo(Node other)
            {
                if (this.ID > other.ID)
                    return 1;
                if (this.ID < other.ID)
                    return -1;
                else return 0;
            }
        }
    }
}
