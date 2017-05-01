using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Species
    {
        public Genome Representative { get { return _members[0]; } }

        private List<Genome> _members = new List<Genome>();

        public Species(Genome representative)
        {
            Add(representative);
        }

        public void Add(Genome g)
        {
            if (g.MemberOf != null)
                g.MemberOf._members.Remove(g);
            g.MemberOf = this;
            g.MemberOf._members.Add(g);
        }


    }
}
