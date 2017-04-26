using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class OutputNode : Node
    {
        public OutputNode(int id) : base(id)
        {
            NodeType = Type.Output;
        }
    }
}
