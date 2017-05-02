using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    abstract class Problem
    {
        public delegate decimal ActivationFunction(decimal input);
        public delegate decimal FitnessFunction(Genome g);
        public delegate decimal[] InputFunction();
        
        public virtual  ActivationFunction InputActivation => (decimal input) => { return input; };
        public abstract ActivationFunction Activation { get; }
        public abstract FitnessFunction Fitness { get; }
        public abstract InputFunction Input { get; }
        public abstract bool HasInput { get; }
    }
}
