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

        public abstract string Name { get; }
        public abstract FitnessFunction FitnessFunc { get; }
        public abstract void Initialize();
    }
}
