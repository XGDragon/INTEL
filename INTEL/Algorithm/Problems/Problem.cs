using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    abstract class Problem
    {
        public const decimal NO_CHANGE_THRESHOLD = 0.001m;

        /// <summary>
        /// Called when node inputs are transformed into outputs.
        /// </summary>
        public delegate decimal ActivationFunction(decimal input);

        /// <summary>
        /// Each call from InputFunction results in an array of outputs from the genome's output nodes. These are put in a
        /// list, where [0] is the output vector from the first output result, and [n] from the last. These outputs are used
        /// to calculate a fitness total.
        /// </summary>
        public delegate decimal FitnessFunction(List<decimal[]> outputs);
        /// <summary>
        /// Returns a predefined array of inputs, where [0] is inserted in the first InputNode, and [n] in the nth Input Node.
        /// When there are fewer Input nodes than inputs, or few inputs than there are Input nodes, then those are ignored.
        /// </summary>
        public delegate decimal[] InputFunction();
        
        public virtual  ActivationFunction InputActivation => (decimal input) => { return input; };
        public abstract ActivationFunction Activation { get; }
        public abstract FitnessFunction Fitness { get; }
        public abstract InputFunction Input { get; }
        public abstract bool HasInput { get; }
    }
}
