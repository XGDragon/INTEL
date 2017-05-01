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

        public override FitnessFunction FitnessFunc => (Genome g) =>
        {
            for (int i = 0; i < _inputPattern.Length; i++) //for each input pattern
            {
                g.Nodes.Inputs(_inputPattern[i]);
                g.Nodes.Activate(XORInputActivation, Node.Type.Bias, Node.Type.Input);
                g.Nodes.Activate(XORActivation, Node.Type.Hidden, Node.Type.Output);

                int no_change_count = 0;
                int index_loop = 0;

                while (no_change_count < g.Nodes.Count && index_loop < 3 * g.Connections.Count)
                {
                    index_loop++;
                    decimal[] outputs = g.Nodes.Outputs();
                    for (int j = 0; j < g.Nodes.Count; j++)
                        g.Nodes[j].ExportOutput();
                    //line 55 of xorexperiment.m
                    //g.Nodes.Activate(XORActivation, Node.Type.Hidden, Node.Type.Output);
                }
            }

            return 0;
        };

        public ActivationFunction XORInputActivation
        {
            get =>
                (decimal input) =>
                {
                    return input;
                };
        }

        public ActivationFunction XORActivation
        {
            get =>
                (decimal input) =>
                {
                    double e = -4.9 * (double)input;
                    return (decimal)(1 / (1 + Math.Exp(e)));
                };
        }


        public override void Initialize()
        {
            Parameter.SetInputs(2);
            Parameter.SetOutputs(1);
        }
    }
}
