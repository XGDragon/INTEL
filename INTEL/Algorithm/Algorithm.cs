using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL.Network
{
    public class Algorithm
    {
        public List<Species> Species { get; private set; }
        public Problem[] Problem { get; private set; }
        public double MaxFitness { get; private set; }             
        public int Generation { get; private set; }
        public int MaxGenerations { get; private set; }
        public Genome Fittest { get; private set; }

        public delegate void GenerationReportHandler(Algorithm.Info info);
        public event GenerationReportHandler GenerationCompleted;

        public Algorithm(ProblemFactory pf, int maxGenerations)
        {
            pf.Initialize();
            Problem = pf.Create();
            MaxFitness = pf.MaxFitness;
            MaxGenerations = maxGenerations;
        }

        public void Run()
        {
            InitializeRun();
            while (MaxGenerations == 0 || Generation++ < MaxGenerations) //best fitness < maxfitness, otherwise stop
            {
                NextGeneration();
                ReportGeneration();
            }
        }

        private void ReportGeneration()
        {
            if (Fittest != null)
                Console.WriteLine("Generation " + Generation + ": Top fitness is at " + Fittest.Fitness);

            GenerationCompleted(new Info(this));
        }

        public override string ToString()
        {
            if (Fittest != null)
                return "Generation " + Generation + ": Top fitness is at " + Fittest.Fitness;
            return base.ToString();
        }

        private void InitializeRun()
        {
            //Initial Population
            List<Genome> initialPopulation = new List<Genome>();
            for (int i = 0; i < Parameter.PopulationSize; i++)
                initialPopulation.Add(new Genome());

            //Speciate
            Species = new List<Species>();
            foreach (Genome g in initialPopulation)
            {
                var match = Species.Find(s => { return (new GenomeComparison(g, s.Representative).Distance < Parameter.SpeciationThreshold); });
                if (match != null)
                    match.AddOffspring(g);
                else
                    Species.Add(new Species(g));
            }

            Generation = 0;
        }

        private void NextGeneration()
        {            
            //Evaluate and check for Stagnation
            foreach (Species s in Species)
            {
                s.EvaluateSpecies(Problem);
                s.CheckIfStagnant();
            }

            //Check for algorithm refocus
            if (Species.Max().CheckIfRefocus())
            {
                Species.Sort();     //from worst to best
                for (int i = 0; i < Species.Count - 2; i++) //exclude the 2 best species
                    Species[i].EliminateSpecies();
            }

            //check if solution found?

            List<Genome> newPopulation = new List<Genome>();
            Species.RemoveAll((Species s) => { return s.ParentCount == 0; }); //remove empty species

            //Get offspring counts
            double sumMeanFitnesses = Species.Sum((Species s) => { return s.CurrentSnapshot.MeanFitness; });
            double overflow = 0;
            foreach (Species s in Species)
            {
                s.CalculateOffspring(ref overflow, sumMeanFitnesses);
                if (s.Elite)
                    newPopulation.Add(s.CurrentSnapshot.FittestGenome); //elitism                
                s.CullTheWeak();
            }
            
            //Crossover & mutation
            foreach (Species s in Species)
            {
                int overall = s.AllowedOffspring - ((s.Elite) ? 1 : 0);
                int crossovers = (int)Math.Round(overall * Parameter.CrossoverPercentage);
                int mutations = overall - crossovers;
                Genome[] selected = s.SelectParents(crossovers, mutations);
                int parent = 0;

                while (crossovers-- > 0)
                {
                    if (Program.R.NextDouble() < Parameter.CrossoverInterspecies && Species.Count > 1)
                        selected[parent] = InterspeciesGenome(s);
                    newPopulation.Add(new Genome(selected[parent++], selected[parent++]));
                }

                while (mutations-- > 0)
                    newPopulation.Add(new Genome(selected[parent++]));
            }

            foreach (Genome g in newPopulation)
            {
                var match = Species.Find(s => { return (new GenomeComparison(g, s.Representative).Distance < Parameter.SpeciationThreshold); });
                if (match != null)
                    match.AddOffspring(g);
                else
                    Species.Add(new Species(g));
            }
            
            Fittest = Species.Max().CurrentSnapshot.FittestGenome;
        }

        private Genome InterspeciesGenome(Species local)
        {
            int[] q = new int[Species.Count - 1];
            int x = 0;
            for (int i = 0; i < q.Length; i++)
                if (Species[i] != local)
                    q[x] = x++;
            return Species[q[Program.R.Next(q.Length)]].RandomParent();
        }

        public class Info
        {
            public string Title { get; private set; }
            public Genome Fittest { get; private set; }
            public Dictionary<int, int> SpeciesMap { get; private set; }
            public int Generation { get; private set; }

            public Info(Algorithm n)
            {
                Title = n.ToString();
                Fittest = n.Fittest;
                Generation = n.Generation;

                SpeciesMap = new Dictionary<int, int>();
                for (int i = 0; i < n.Species.Count; i++)
                    SpeciesMap.Add(n.Species[i].ID, n.Species[i].OffspringCount);
            }
        }
    }
}
