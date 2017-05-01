using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class InputNode : Node
    {
        public override decimal State { get => base.State; set => base.State = (NodeType == Type.Bias) ? 1 : value; }

        public InputNode(int id, bool bias) : base(id)
        {
            NodeType = (bias) ? Type.Bias : Type.Input;
        }
    }
}
