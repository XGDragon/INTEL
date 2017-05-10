using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL.Network
{
    public class XORProblem : Problem
    {
        private int _patternEnumerator = 0;

        private double[][] _inputPattern = new double[4][]
            {
                new double[] { 0, 0 },
                new double[] { 0, 1 },
                new double[] { 1, 0 },
                new double[] { 1, 1 }
            };

        private double[][] _outputPattern = new double[4][]
            {
                new double[] { 0 },
                new double[] { 1 },
                new double[] { 1 },
                new double[] { 0 }
            };
        
        public override ActivationFunction Activation => (double input) => 
        {
            double e = -4.9 * (double)input; //Parameter used by the paper for XOR problem, pp 112
            return (double)(1 / (1 + Math.Exp(e)));
        };

        public override FitnessFunction Fitness => (List<double[]> outputs) =>
        {
            _patternEnumerator = 0; //reset the enumerator for future use of this problem instance

            double totalFitness = 0;
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
