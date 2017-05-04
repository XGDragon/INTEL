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
        public List<Connection> Connections = new List<Connection>();
        
        public decimal Fitness { get; private set; }
        public Species MemberOf { get; set; }

        public Genome()
        {
            Nodes = new NodeCollection();

            Nodes.Add(new Node(0, Node.Type.Output));
            Nodes.Add(new Node(1, Node.Type.Bias));
            for (int i = 0; i < Parameter.MaxInputNodes; i++)
                Nodes.Add(new Node(i + 2, Node.Type.Input));
            //no hidden in initialization

            Connections.Add(Nodes[1].Connect(Nodes[0]));
            for (int i = 0; i < Parameter.InputNodes && i < Parameter.MaxInputNodes; i++)
                Connections.Add(Nodes[i + 2].Connect(Nodes[0]));
        }

        public Genome(Genome mutate)
        {
            Nodes = new NodeCollection(mutate.Nodes);
            //connect nodes, condition at line 148: size(parent2.nodegenes,2) > 4 & size(parent1.nodegenes,2) > 4 & size(parent2.nodegenes,2) ~= size(parent1.nodegenes,2)
        }

        public Genome(Genome parent1, Genome parent2)
        {
            Nodes = new NodeCollection(parent1.Nodes, parent2.Nodes);
            //need to connect nodes; need to find matching/disjoint/excess. perhaps by going node by node?
        }

        public void EvaluateFitness(Problem[] problems)
        {
            Fitness = 0;
            for (int i = 0; i < problems.Length; i++)
            {
                Problem p = problems[i];
                List<decimal[]> outputs = new List<decimal[]>();

                while (p.HasInput)
                {
                    Nodes.Accept(p.Input());

                    Nodes.Activate(p.InputActivation, Node.Type.Bias, Node.Type.Input);
                    Nodes.Activate(p.Activation, Node.Type.Hidden, Node.Type.Output);

                    int no_change_count = 0;
                    int index_loop = 0;

                    while (no_change_count < Nodes.Count && index_loop < 3 * Connections.Count)
                    {
                        index_loop++;
                        decimal[] old_outputs = Nodes.AllOutputs();
                        for (int j = 0; j < Nodes.Count; j++)
                            Nodes[j].Export();

                        Nodes.Activate(p.Activation, Node.Type.Hidden, Node.Type.Output);

                        //no_change_count=sum(abs(population(index_individual).nodegenes(4,:)-vector_node_state)<no_change_threshold); 
                        //%check for all nodes where the node output state has changed by less than no_change_threshold since last 
                        //iteration through all the connection genes
                        decimal[] new_outputs = Nodes.AllOutputs();
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

        public class ConnectionComparison
        {
            public ConnectionComparison(Genome a, Genome b)
            {

            }
        }
        
        //public class Comparison
        //{
        //    public enum Group { Matching, Disjoint, Excess }
        //    public enum Type { Stronger, Weaker }

        //    private Dictionary<Type, Dictionary<Group, List<Connection>>> _dict = new Dictionary<Type, Dictionary<Group, List<Connection>>>();

        //    private Genome _stronger { get { return (_a > _b) ? _a : _b; } }
        //    private Genome _weaker { get { return (_a > _b) ? _b : _a; } }
            
        //    private Genome _a;
        //    private Genome _b;

        //    private List<Connection> _matching;
        //    private List<Connection> _disjoint;
        //    private List<Connection> _excess;       

        //    public Comparison(Genome a, Genome b)
        //    {
        //        _a = a;
        //        _b = b;

        //        List<Connection> larger = (a.Connections.Count > b.Connections.Count) ? a.Connections : b.Connections;
        //    }

        //    public List<Connection> Get(Type t, Group g) { return _dict[t][g]; }

            
        //}
    }
}
