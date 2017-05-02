using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class XORProblem : Problem
    {
        public override string Name { get { return "XOR Problem"; } }

        private int _patternEnumerator = 0;

        private decimal[][] _inputPattern = new decimal[4][]
            {
                new decimal[2] { 0, 0 },
                new decimal[2] { 0, 1 },
                new decimal[2] { 1, 0 },
                new decimal[2] { 1, 1 }
            };

        private decimal[] _outputPattern = new decimal[4]
            {
                0,
                0,
                1,
                1
            };
        
        public override ActivationFunction Activation => (decimal input) => 
        {
            double e = -4.9 * (double)input;
            return (decimal)(1 / (1 + Math.Exp(e)));
        };

        public override FitnessFunction Fitness => throw new NotImplementedException();

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

        public override Problem[] Initialize()
        {
            Parameter.SetInputs(2);
            Parameter.SetOutputs(1);

            return new Problem[1] { this };
        }
    }
}
