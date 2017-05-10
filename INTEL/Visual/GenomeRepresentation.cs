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

        private Dictionary<INTEL.Network.Node, NodeRepresentation> _nodes = new Dictionary<Network.Node, NodeRepresentation>();        
        private List<(NodeRepresentation a, NodeRepresentation b)> _connections = new List<(NodeRepresentation a, NodeRepresentation b)>();

        public GenomeRepresentation(INTEL.Network.Genome g)
        {
            InitializeComponent();
            _genome = g;

            var nodes = _genome.Nodes.List;
            for (int i = 0; i < nodes.Count; i++)
                _nodes.Add(nodes[i], new NodeRepresentation(nodes[i]));
            
            var connections = _genome.Nodes.Connections.ToArray();
            for (int i = 0; i < connections.Length; i++)
                _connections.Add((_nodes[connections[i].Value.From], _nodes[connections[i].Value.To]));

            Paint += GenomeRepresentation_Paint;
            Invalidate();
        }

        private void GenomeRepresentation_Paint(object sender, PaintEventArgs e)
        {
            var nd = _nodes.Values.OrderBy(n => n.Depth).ToArray(); //low to high depth
            int d = 0;
            double th = nd[0].Depth;
            int maxX = Width - nd[0].Width;
            while (d < nd.Length)
            {
                while (nd[d++].Depth == th);
                for (int i = 0; i < d; i++)
                {
                    //d = count of depth buddies
                    Controls.Add(nd[i]);
                    nd[i].Location = new Point(0, DepthToY(nd[i]));
                }
                th = nd[d].Depth;
            }
        }

        private int DepthToY(NodeRepresentation nr)
        {         
            var maxH = Height - nr.Height;
            return maxH - (int)Math.Round(nr.Depth * maxH);
        }
    }
}
