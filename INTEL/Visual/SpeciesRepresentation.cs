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
    public partial class SpeciesRepresentation : UserControl
    {
        public static int ParentWidth = 150; 

        private List<Line> _lines = new List<Line>();

        public SpeciesRepresentation(INTEL.Network.Algorithm.Info info, int popPerPixel)
        {
            InitializeComponent();
            this.Width = ParentWidth;

            int x = 0;
            var s = info.SpeciesMap.OrderBy(i => i.Key).ToArray(); //low ID to high ID.
            for (int j = 0; j < s.Length; j++)
            {
                int pop = s[j].Value / popPerPixel;
                Pen p = new Pen(Color.FromKnownColor((KnownColor)((s[j].Key) % 167) + 28));
                _lines.Add(new Line(p, x, x + pop));
                x += pop;
            }

            this.Paint += SpeciesRepresentation_Paint;
            this.Invalidate();
        }

        private void SpeciesRepresentation_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < _lines.Count; i++)
            {
                Rectangle rect = new Rectangle(_lines[i].A, new Size(_lines[i].B.X, Height));
                Brush b = new SolidBrush(_lines[i].Pen.Color);
                e.Graphics.FillRectangle(b, rect);
                //e.Graphics.DrawLine(_lines[i].Pen, _lines[i].A, _lines[i].B);
                //e.Graphics.DrawLine(_lines[i].Pen, _lines[i].A, _lines[i].B);
            }
        }

        private struct Line
        {
            public Pen Pen { get; private set; }
            public Point A { get; private set; }
            public Point B { get; private set; }

            public Line(Pen p, int x1, int x2)
            {
                Pen = p;
                A = new Point(x1, 0);
                B = new Point(x2, 0);
            }
        }
    }
}
