using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INTEL
{
    public partial class DataVisual : Form
    {
        private Algorithm _algorithm;

        public DataVisual(Algorithm a)
        {
            _algorithm = a;
            InitializeComponent();
        }

        public void AlgorithmUpdated()
        {
            label1.Text = _algorithm.Generation.ToString();
        }
    }
}
