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

        private decimal[,] _inputPattern = new decimal[4, 2]
            {
                { 0,0 },
                { 0,1 },
                { 1,0 },
                { 1,1 }
            };

        private decimal[] _outputPattern = new decimal[4]
            {
                0,
                0,
                1,
                1
            };

        public override decimal EvaluateFitness(Genome g)
        {
            g.Nodes.Inputs.ForEach((Node n) => { n.State = 0; });   //set all inputs to 0 (bias cannot be set to anything other than 1)


            return 0;
        }

        public override void Initialize()
        {
            Parameter.SetInputs(2);
            Parameter.SetOutputs(1);
        }
    }
}
