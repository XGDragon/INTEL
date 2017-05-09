using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    abstract class ProblemFactory
    {
        public abstract string Name { get; }
        public double MaxFitness { get; protected set; }

        public abstract void Initialize();
        public abstract Problem[] Create();

        protected string InitiatilizingText { get { return "\nInitializing " + Name; } }
    }
}
