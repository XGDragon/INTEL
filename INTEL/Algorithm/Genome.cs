using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Genome : IComparable<Genome>
    {
        //candidates for privatization
        public NodeCollection Nodes { get; private set; }
        
        public double Fitness { get; private set; }
        public Species MemberOf { get; set; }

        public Genome()
        {
            Nodes = new NodeCollection();

            Nodes.Add(new Node(0, Node.Type.Output));
            Nodes.Add(new Node(1, Node.Type.Bias));
            for (int i = 0; i < Parameter.MaxInputNodes; i++)
                Nodes.Add(new Node(i + 2, Node.Type.Input));

            Nodes.Connect(Nodes[1], Nodes[0]);
            for (int i = 0; i < Parameter.InputNodes && i < Parameter.MaxInputNodes; i++)
                Nodes.Connect(Nodes[i + 2], (Nodes[0]));            
        }

        public Genome(Genome mutate)
        {
            Nodes = new NodeCollection(mutate);
            Nodes.Mutate();
        }

        public Genome(Genome parent1, Genome parent2)
        {
            Nodes = new NodeCollection(parent1, parent2);
            Nodes.Mutate();
        }

        public void EvaluateFitness(Problem[] problems)
        {
            Fitness = 0;
            for (int i = 0; i < problems.Length; i++)
            {
                Problem p = problems[i];
                List<double[]> outputs = new List<double[]>();

                while (p.HasInput)
                {
                    Nodes.Accept(p.Input());

                    Nodes.Activate(p.InputActivation, Node.Type.Bias, Node.Type.Input);
                    Nodes.Activate(p.Activation, Node.Type.Hidden, Node.Type.Output);

                    int no_change_count = 0;
                    int index_loop = 0;

                    while (no_change_count < Nodes.Count && index_loop < 3 * Nodes.Connections.Count)
                    {
                        index_loop++;
                        double[] old_outputs = Nodes.AllOutputs();
                        for (int j = 0; j < Nodes.Count; j++)
                            Nodes[j].Export();

                        Nodes.Activate(p.Activation, Node.Type.Hidden, Node.Type.Output);
                        
                        double[] new_outputs = Nodes.AllOutputs();
                        for (int j = 0; j < old_outputs.Length; j++)
                            no_change_count += (Math.Abs(new_outputs[j] - old_outputs[j]) < Problem.NO_CHANGE_THRESHOLD) ? 1 : 0;
                    }

                    outputs.Add(Nodes.Outputs());
                }

                Fitness += p.Fitness(outputs);
            }           

            //actual problem:
            //list of opponents. per opponent, play n games (filling up inputs with results). output must be the counter strategy.
            //suggestion for output: output 1 - 4 is resp. CC CD DC DD results from last round. depending on last round, use mixed strategy of output x
            //each strat has a counterstrategy vector size 4. AllD counterstrategy is { 0, 0, 0, 0 }. AllC and TFT counterstrategy is { 1, 1, 1, 1 }. random is { .5, .5, .5, .5 }
            //each game fitness is calculated by comparing current output vector with counterstrategy vector. smaller difference > higher fitness, possibly with greater weight for earlier games.
            //SEE PAPER FOR IDEAS ON FITNESS FUNCTIONS ETC, PAGE 112
        }

        public int CompareTo(Genome other)
        {
            if (this > other)
                return 1;
            if (this < other)
                return -1;
            else return 0;
        }

        public static bool operator >(Genome a, Genome b) { return a.Fitness > b.Fitness; }
        public static bool operator <(Genome a, Genome b) { return a.Fitness < b.Fitness; }        
    }
}
