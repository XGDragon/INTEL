using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Species
    {
        private List<Genome> _members = new List<Genome>();

        public void Add(Genome g)
        {
            _members.Add(g);
        }
    }
}
