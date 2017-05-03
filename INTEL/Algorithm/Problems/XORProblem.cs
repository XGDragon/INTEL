using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class XORProblem : Problem
    {
        private int _patternEnumerator = 0;

        private decimal[][] _inputPattern = new decimal[4][]
            {
                new decimal[] { 0, 0 },
                new decimal[] { 0, 1 },
                new decimal[] { 1, 0 },
                new decimal[] { 1, 1 }
            };

        private decimal[][] _outputPattern = new decimal[4][]
            {
                new decimal[] { 0 },
                new decimal[] { 1 },
                new decimal[] { 1 },
                new decimal[] { 0 }
            };
        
        public override ActivationFunction Activation => (decimal input) => 
        {
            double e = -4.9 * (double)input; //Parameter used by the paper for XOR problem, pp 112
            return (decimal)(1 / (1 + Math.Exp(e)));
        };

        public override FitnessFunction Fitness => (List<decimal[]> outputs) =>
        {
            _patternEnumerator = 0; //reset the enumerator for future use of this problem instance

            decimal totalFitness = 0;
            for (int i = 0; i < outputs.Count; i++)
                totalFitness += Math.Abs(_outputPattern[i][0] - outputs[i][0]);

            totalFitness = 4 - totalFitness; //(4 - f) ^ 2
            return totalFitness * totalFitness;
        };

        public override InputFunction Input => () =>
        {
            return _inputPattern[_patternEnumerator++];
        };

        public override bool HasInput
        {
            get
            {
                return _patternEnumerator < _inputPattern.Length;
            }
        }
    }
}
