using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    public class Parameter
    {
        private Parameter(string name, decimal v, string tooltip = "")
        {
            _name = name;
            Value = v;
            if (tooltip.Length > 0)
                Tooltip = " #" + tooltip;

            List.Add(this);
        }

        public decimal Value { get; private set; }
        public string Tooltip { get; private set; }
        private string _name;
        public override string ToString() { return _name; }

        //_______________________________________________________________//

        private const string PARAMFILE = "params.ini";
        public static void Initialize()
        {
            if (System.IO.File.Exists(PARAMFILE))
            {
                StreamReader sr = new StreamReader(PARAMFILE);
                for (int i = 0; i < List.Count; i++)
                {
                    decimal d;
                    if (decimal.TryParse(sr.ReadLine().Split('=')[1], out d))
                        List[i].Value = d;
                }
                sr.Close();
            }
            else
            {
                StreamWriter sw = new StreamWriter(PARAMFILE);
                for (int i = 0; i < List.Count; i++)
                    sw.WriteLine(i + ":" + List[i]._name + "=" + List[i].Value);
                sw.Close();
            }
        }

        public static void SetInputs(int i)
        {
            _inputNodes.Value = i;
            _maxInputNodes.Value = i;
        }

        public static void SetOutputs(int i)
        {
            _outputNodes.Value = i;
        }

        public static List<Parameter> List = new List<Parameter>();
        public static Dictionary<int, string> ListDecorator = new Dictionary<int, string>()
            {
                    { 0, "\nPopulation Parameters"},
                    { 4, "\nSpeciation Parameters"},
                    { 8, "\nReproduction Parameters\n- Stagnation + Refocus"},
                    { 12, "\n- Initial Setup"},
                    { 15, "\n- Selection"},
                    { 16, "\n- Crossover"},
                    { 19, "\n- Mutation"},
            };

        //Population
        private static Parameter _populationSize = new Parameter("Population Size", 150);
        public static decimal PopulationSize { get { return _populationSize.Value; } }

        private static Parameter _inputNodes = new Parameter("Connected Input Nodes", 2, "if less than max, we start with a subset and let evolution decide which ones are necessary");
        public static decimal InputNodes { get { return _inputNodes.Value; } }

        private static Parameter _maxInputNodes = new Parameter("Max Input Nodes", 10);
        public static decimal MaxInputNodes { get { return _maxInputNodes.Value; } }

        private static Parameter _outputNodes = new Parameter("Output Nodes", 1);
        public static decimal OutputNodes { get { return _outputNodes.Value; } }

        //Speciation
        private static Parameter _c1 = new Parameter("c1", 1.0m, "Adjusts the importance of excess genes when calculating species distance");
        public static decimal c1 { get { return _c1.Value; } }

        private static Parameter _c2 = new Parameter("c2", 1.0m, "Adjusts the importance of disjoint genes when calculating species distance");
        public static decimal c2 { get { return _c2.Value; } }

        private static Parameter _c3 = new Parameter("c3", 0.4m, "Adjusts the importance of weights when calculating species distance");
        public static decimal c3 { get { return _c3.Value; } }

        private static Parameter _speciationThreshold = new Parameter("Threshold", 3);
        public static decimal SpeciationThreshold { get { return _speciationThreshold.Value; } }

        //Reproduction
        //Stagnation + Refocus
        private static Parameter _stagnationThreshold = new Parameter("Stagnation Threshold", 0.02m, "threshold to judge if a species is in stagnation");
        public static decimal StagnationThreshold { get { return _stagnationThreshold.Value; } }

        private static Parameter _stagnationGenerations = new Parameter("Stagnation Generations", 15, "if max fitness of species has stayed within stagnation.threshold in the last stagnation.number_generation generations, all its fitnesses will be reduced to 0 (this kills the species)");
        public static decimal StagnationGenerations { get { return _stagnationGenerations.Value; } }

        private static Parameter _refocusThreshold = new Parameter("Refocus Threshold", 0.02m, "threshold to judge if a species needs refocusing");
        public static decimal RefocusThreshold { get { return _refocusThreshold.Value; } }

        private static Parameter _refocusGenerations = new Parameter("Refocus Generations", 20, "if maximum overall fitness of population doesn't change within threhold for this number of generations, only the top two species are allowed to reproduce");
        public static decimal RefocusGenerations { get { return _refocusGenerations.Value; } }

        //Initial Setup
        private static Parameter _killPercentage = new Parameter("Kill Percentage", 0.2m, "the percentage of each species which will be eliminated (lowest performing individuals)");
        public static decimal KillPercentage { get { return _killPercentage.Value; } }

        private static Parameter _killAmount = new Parameter("Kill Amount", 5, "the above percentage for eliminating individuals will only be used in species which have more individuals than min_number_for_kill");
        public static decimal KillAmount { get { return _killAmount.Value; } }

        private static Parameter _elitism = new Parameter("Elitism Threshold", 5, "species which have individuals equal or greater than this threshold will have their best individual copied unchanged into the next generation");
        public static decimal ElitismThreshold { get { return _elitism.Value; } }

        //Selection
        private static Parameter _selectionPressure = new Parameter("Pressure", 2.0m, "Number between 1.1 and 2.0, determines selective pressure towards most fit individual of species");
        public static decimal SelectionPressure { get { return _selectionPressure.Value; } }

        //Crossover
        private static Parameter _crossoverPercentage = new Parameter("Percentage", 0.8m, "percentage governs the way in which new population will be composed from old population.");
        public static decimal CrossoverPercentage { get { return _crossoverPercentage.Value; } }

        private static Parameter _crossoverInterspecies = new Parameter("Interspecies", 0.0m, "if crossover has been selected, this probability governs the intra/interspecies parent composition being used");
        public static decimal CrossoverInterspecies { get { return _crossoverInterspecies.Value; } }

        private static Parameter _crossoverMultipoint = new Parameter("Multipoint", 0.6m, "standard-crossover in which matching connection genes are inherited randomly from both parents. In the (1-multipoint) cases, weights of the new connection genes are the mean of the corresponding parent genes");
        public static decimal CrossoverMultipoint { get { return _crossoverMultipoint.Value; } }

        public const decimal CrossoverDisable = 0.75m; //constant?

        //Mutation
        private static Parameter _mutationAddNode = new Parameter("Add Node Probability", 0.03m);
        public static decimal MutationAddNode { get { return _mutationAddNode.Value; } }

        private static Parameter _mutationAddConnection = new Parameter("Add Connection Probability", 0.05m);
        public static decimal MutationAddConnection { get { return _mutationAddConnection.Value; } }

        private static Parameter _mutationRecurrency = new Parameter("Recurrency Probability", 0.0m, "if we are in add_connection_mutation, this governs if a recurrent connection is allowed. Note: this will only activate if the random connection is a recurrent one, otherwise the connection is simply accepted. If no possible non-recurrent connections exist for the current node genes, then for e.g. a probability of 0.1, 9 times out of 10 no connection is added.");
        public static decimal MutationRecurrency { get { return _mutationRecurrency.Value; } }

        private static Parameter _mutationChangeWeight = new Parameter("Mutate Weight Probability", 0.9m);
        public static decimal MutationChangeWeight { get { return _mutationChangeWeight.Value; } }

        private static Parameter _mutationMaxWeightRange = new Parameter("Weight Cap", 8, "weights will be restricted from -weight_cap to +weight_cap");
        public static decimal MutationMaxWeightRange { get { return _mutationMaxWeightRange.Value; } }

        private static Parameter _mutationInitWeightRange = new Parameter("Weight Initialization Range", 5, "random distribution with width mutation.weight_range, centered on 0. mutation range of 5 will give random distribution from -2.5 to 2.5");
        public static decimal MutationInitWeightRange { get { return _mutationInitWeightRange.Value; } }

        private static Parameter _mutationReEnable = new Parameter("Node Re-enable Probability", 0.25m, "Probability of a connection gene being reenabled in offspring if it was inherited disabled");
        public static decimal MutationReEnable { get { return _mutationReEnable.Value; } }
    }
}
