using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class HiddenNode : Node
    {
        public HiddenNode(int id) : base(id)
        {
            NodeType = Type.Hidden;
        }
    }
}
