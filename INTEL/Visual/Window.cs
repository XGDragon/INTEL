using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace INTEL.Visual
{
    public partial class Window : Form
    {
        private const int MAX_SPECIES_WIDTH = 500;

        private List<GenomeRepresentation> _representations = new List<GenomeRepresentation>();
        private int popPerPixel = 1;

        public Window()
        {
            InitializeComponent();
            
            ResizeSpeciesMap();
        }

        public void AlgorithmUpdated(INTEL.Network.Algorithm.Info info)
        {
            this.Text = info.Title;

            AddSpeciesToRepresentation(info);
            AddGenomeToRepresentation(info);
        }

        private void ResizeSpeciesMap()
        {
            popPerPixel = (int)Math.Ceiling(Parameter.PopulationSize / MAX_SPECIES_WIDTH);
            int w = SpeciesPanel.Width;
            SpeciesPanel.Width = ((int)Parameter.PopulationSize / popPerPixel);
            this.Width -= w - SpeciesPanel.Width;
            SpeciesRepresentation.ParentWidth = SpeciesPanel.Width;
        }

        private void AddSpeciesToRepresentation(INTEL.Network.Algorithm.Info info)
        {
            var c = SpeciesPanel.Controls;
            c.Add(new SpeciesRepresentation(info, popPerPixel));
            for (int i = 0; i < c.Count; i++)
                c[i].Height = SpeciesPanel.Height / c.Count;
        }

        private void AddGenomeToRepresentation(INTEL.Network.Algorithm.Info info)
        {
            _representations.Add(new GenomeRepresentation(info.Fittest));
            GenomeCounter.Maximum = _representations.Count;
            GenomeCounter.Value = _representations.Count;
        }

        private void GenomeCounter_ValueChanged(object sender, EventArgs e)
        {
            GenomePanel.Controls.Clear();
            GenomePanel.Controls.Add(_representations[(int)GenomeCounter.Value - 1]);
        }
    }
}
