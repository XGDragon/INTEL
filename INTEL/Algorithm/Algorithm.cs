using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Algorithm
    {
        public static Random R { get; private set; }

        public Problem[] Problem { get; private set; }
        public decimal MaxFitness { get; private set; }

        public List<Genome> Population = new List<Genome>();
        public List<Species> Species = new List<Species>();
        //public List<Species> Species = new List<Species>(); //issues with who controls who
        
        public int Generation { get; private set; }

        public Algorithm(ProblemFactory pf)
        {
            R = new Random();

            pf.Initialize();
            Problem = pf.Create();
            MaxFitness = pf.MaxFitness;
        }

        public void Run(int maxGenerations)
        {
            Initialize();
            Console.WriteLine("Generation " + Generation);

            while (Generation++ < maxGenerations) //best fitness < maxfitness, otherwise stop
            {
                Console.WriteLine("Generation " + Generation);
                NextGeneration();
            }
        }

        private void Initialize()
        {
            //Initial Population
            for (int i = 0; i < Parameter.PopulationSize; i++)
                Population.Add(new Genome());

            //Speciate (potentially encapsulate)
            Species.Add(new Species(Population[0]));
            for (int i = 1; i < Population.Count; i++)
            {
                bool assigned_existing_species = false;
                bool new_species = false;
                int index_species = 0;

                while (!assigned_existing_species && !new_species)
                {
                    //at initialization, we need not worry about c1,c2, because there are no disjoint or excess connections
                    decimal weightDifferenceTotal = 0;
                    for (int j = 0; j < Population[i].Connections.Count; j++)
                        weightDifferenceTotal += Math.Abs(Population[i].Connections[j] - Species[index_species].Representative.Connections[j]);

                    decimal distance = (Parameter.c3 * weightDifferenceTotal) / Population[i].Connections.Count;
                    if (distance < Parameter.SpeciationThreshold)
                    {
                        Species[index_species].Add(Population[i]);
                        assigned_existing_species = true;
                    }
                    index_species++;
                    if (index_species > Species.Count - 1 && !assigned_existing_species)
                        new_species = true;
                }

                if (new_species)
                    Species.Add(new Species(Population[i]));
            }

            Generation = 0;
        }

        private void NextGeneration()
        {
            foreach (Genome g in Population)
                g.EvaluateFitness(Problem);
            
            foreach (Species s in Species)
            {
                s.UpdateData(Generation);
                s.EliminateSpecies = s.CheckIfStagnant();
            }

            if (Species.Max().CheckIfRefocus())
            {
                Species.Sort();     //from worst to best
                for (int i = 0; i < Species.Count - 2; i++) //exclude the 2 best species
                    Species[i].EliminateSpecies = (Species[i].CurrentData.MaxFitness > 0);
            }

            //check if solution found?

            List<Genome> newPopulation = new List<Genome>();
            Species.RemoveAll((Species s) => { return s.Count == 0; });

            decimal avgFitnessAllSpecies = Species.Average((Species s) => { return s.CurrentData.MeanFitness; });
            decimal overflow = 0;
            foreach (Species s in Species)
            {
                s.CalculateOffspring(ref overflow, avgFitnessAllSpecies);
                s.Elite = (s.Offspring > 0 && s.Count > Parameter.ElitismThreshold);
                if (s.Elite)
                    newPopulation.Add(s.CurrentData.FittestGenome); //elitism
                if (s.Count > Parameter.KillPercentage && Math.Ceiling(s.Count * (1 - Parameter.KillPercentage)) > 2)
                    s.CullTheWeak();

                s.ShuffleRepresentative();
            }

            foreach (Species s in Species)
            {
                int overall = s.Offspring - ((s.Elite) ? 1 : 0);
                int crossovers = (int)Math.Round(overall * Parameter.CrossoverPercentage);
                int mutations = overall - crossovers;
                var selected = s.Select(crossovers, mutations);
                int parent = 0;

                while (crossovers-- > 0)
                {
                    if ((decimal)R.NextDouble() < Parameter.CrossoverInterspecies)
                    {
                        //CrossoverInterspecies
                    }
                    else
                    {
                        //Crossover within Species
                        newPopulation.Add(new Genome(selected[parent++], selected[parent++]));
                    }
                }

                while (mutations-- > 0)
                {
                    newPopulation.Add(new Genome(selected[parent++]));
                }
            }
        }
    }
}
