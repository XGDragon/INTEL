using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    abstract class ProblemFactory
    {
        public abstract void Initialize();
        public abstract Problem[] Create();
        public abstract string Name { get; }
    }
}
