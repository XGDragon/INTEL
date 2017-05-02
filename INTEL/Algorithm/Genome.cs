using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Genome
    {
        public NodeCollection Nodes = new NodeCollection();
        public List<Connection> Connections = new List<Connection>();

        public decimal Fitness { get; private set; }
        public Species MemberOf { get; set; }

        public Genome()
        {
            Nodes.Add(new OutputNode(0));
            Nodes.Add(new BiasNode(1));
            for (int i = 0; i < Parameter.MaxInputNodes; i++)
                Nodes.Add(new InputNode(i + 2));
            //no hidden in initialization

            Connections.Add(Nodes[1].Connect(Nodes[0]));
            for (int i = 0; i < Parameter.InputNodes && i < Parameter.MaxInputNodes; i++)
                Connections.Add(Nodes[i + 2].Connect(Nodes[0]));
        }

        public void EvaluateFitness(Problem p)
        {
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
                    decimal[] old_outputs = Nodes.Outputs(); //remember all outputs
                    for (int j = 0; j < Nodes.Count; j++)
                        Nodes[j].Export();

                    Nodes.Activate(p.Activation, Node.Type.Hidden, Node.Type.Output);
                    //set hidden/output inputs back to zero

                    //no_change_count=sum(abs(population(index_individual).nodegenes(4,:)-vector_node_state)<no_change_threshold); 
                    //%check for all nodes where the node output state has changed by less than no_change_threshold since last 
                    //iteration through all the connection genes
                    decimal[] new_outputs = Nodes.Outputs();
                    //for (int i = 0; i < outputs.Length)
                }
            }

            /*            for (int i = 0; i < _inputPattern.Length; i++) //for each input pattern
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
            }*/




            //actual problem:
            //list of opponents. per opponent, play n games (filling up inputs with results). output must be the counter strategy.
            //suggestion for output: output 1 - 4 is resp. CC CD DC DD results from last round. depending on last round, use mixed strategy of output x
            //each strat has a counterstrategy vector size 4. AllD counterstrategy is { 0, 0, 0, 0 }. AllC and TFT counterstrategy is { 1, 1, 1, 1 }. random is { .5, .5, .5, .5 }
            //each game fitness is calculated by comparing current output vector with counterstrategy vector. smaller difference > higher fitness, possibly with greater weight for earlier games.
            //Fitness = ff(this);
        }


        public static bool operator >(Genome a, Genome b) { return a.Fitness > b.Fitness; }
        public static bool operator <(Genome a, Genome b) { return a.Fitness < b.Fitness; }
        
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
