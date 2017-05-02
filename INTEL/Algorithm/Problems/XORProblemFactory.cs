using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class XORProblemFactory : ProblemFactory
    {
        public override string Name { get { return "XOR Problem"; } }

        public override Problem[] Create()
        {
            return new XORProblem[1] { new XORProblem() };
        }

        public override void Initialize()
        {
            Parameter.SetInputs(2);
            Parameter.SetOutputs(1);
        }
    }
}
