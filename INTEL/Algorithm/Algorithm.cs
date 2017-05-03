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

        private int _generation = 0;
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

            //sum average fitness of all species
            decimal avgFitnessAllSpecies = Species.Average((Species s) => { return s.CurrentData.MeanFitness; });
            List<Genome> newPopulation = new List<Genome>();

            decimal overflow = 0;
            int index_individual = 0;
            List<Species> matrix_existing_and_propagating_species = new List<Species>();
            foreach (Species s in Species)
            {
                if (s.Count > 0)
                {
                    decimal number_offspring = s.CurrentData.MeanFitness / avgFitnessAllSpecies * Parameter.PopulationSize;
                    overflow += number_offspring - Math.Truncate(number_offspring);
                    number_offspring = (overflow > 1) ? Math.Ceiling(number_offspring) : Math.Floor(number_offspring);

                    if (number_offspring > 0)
                    {
                        matrix_existing_and_propagating_species.Add(s);
                        if (s.Count > Parameter.ElitismThreshold)
                        {
                            index_individual++;
                            newPopulation.Add(s.CurrentData.FittestGenome);
                        }
                    }

                    if (s.Count > Parameter.KillPercentage && Math.Ceiling(s.Count * (1 - Parameter.KillPercentage)) > 2)
                    {
                        //get from this species all indivudals and their fitnesses..
                        //sort this list from shit tier to S tier
                        //destroy Parameter.KillPercentage bottom tier
                    }
                }
            }

        }
        /*
          


        */
    }
}
