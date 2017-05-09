using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Algorithm
    {
        public Problem[] Problem { get; private set; }
        public double MaxFitness { get; private set; }
        
        public List<Species> Species = new List<Species>();
        
        public int Generation { get; private set; }
        public Genome Fittest { get; private set; }

        public Algorithm(ProblemFactory pf)
        {
            pf.Initialize();
            Problem = pf.Create();
            MaxFitness = pf.MaxFitness;
        }

        public void Run(int maxGenerations)
        {
            Initialize();
            AnnounceFittestGenome();

            while (Generation++ < maxGenerations) //best fitness < maxfitness, otherwise stop
            {
                AnnounceFittestGenome();
                NextGeneration();
            }
        }

        private void AnnounceFittestGenome()
        {
            Console.WriteLine("Generation " + Generation + ": Top fitness is at " + Fittest.Fitness);
        }

        private void Initialize()
        {
            //Initial Population
            List<Genome> initialPopulation = new List<Genome>();
            for (int i = 0; i < Parameter.PopulationSize; i++)
                initialPopulation.Add(new Genome());

            //Speciate
            foreach (Genome g in initialPopulation)
            {
                var match = Species.Find(s => { return (new GenomeComparison(g, s.Representative).Distance < Parameter.SpeciationThreshold); });
                if (match != null)
                    match.AddOffspring(g);
                else
                    Species.Add(new INTEL.Species(g));
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
            double avgFitnessAllSpecies = Species.Average((Species s) => { return s.CurrentSnapshot.MeanFitness; });
            double overflow = 0;
            foreach (Species s in Species)
            {
                s.CalculateOffspring(ref overflow, avgFitnessAllSpecies); //WRONG VALUE
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
                    Species.Add(new INTEL.Species(g));
            }


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
    }
}
