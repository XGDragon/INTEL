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
    public partial class NodeRepresentation : UserControl
    {
        public double Depth { get { return _node.Depth; } }
        public int ID { get { return _node.ID; } }

        private INTEL.Network.Node _node;

        public NodeRepresentation(INTEL.Network.Node node)
        {
            InitializeComponent();
            _node = node;

            NodeTypeLabel.Text = _node.ID + "\n" + _node.NodeType.ToString();

            Paint += NodeRepresentation_Paint;
            Invalidate();
        }

        private void NodeRepresentation_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawEllipse(Pens.DarkSlateGray, 0, 0, Width, Height);
        }
    }
}
