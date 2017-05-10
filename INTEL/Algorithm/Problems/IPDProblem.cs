using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL.Network
{
    public class IPDProblem : Problem
    {
        public override ActivationFunction Activation => throw new NotImplementedException();

        public override FitnessFunction Fitness => throw new NotImplementedException();

        public override InputFunction Input => throw new NotImplementedException();

        public override bool HasInput => throw new NotImplementedException();
    }
}
