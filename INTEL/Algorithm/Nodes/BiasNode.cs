using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class BiasNode : Node
    {
        private const decimal BIAS_INPUT = 1;
        public override decimal Input { get => BIAS_INPUT; protected set => base.Input = value; }

        public BiasNode(int id) : base(id)
        {
            NodeType = Type.Bias;
        }
    }
}
