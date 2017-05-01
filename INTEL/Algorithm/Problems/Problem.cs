using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    abstract class Problem
    {
        public abstract string Name { get; }
        public abstract decimal EvaluateFitness(Genome g);
        public abstract void Initialize();
    }
}
