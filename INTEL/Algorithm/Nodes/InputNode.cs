using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class InputNode : Node
    {
        public InputNode(int id) : base(id)
        {
            NodeType = Type.Input;
        }

        public void SetInput(decimal input)
        {
            Input = input;
        }
    }
}
