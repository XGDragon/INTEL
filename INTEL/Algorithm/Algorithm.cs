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
        public int Generation { get { return _generation; } private set { Console.WriteLine("Generation " + _generation.ToString()); _generation = value; } }

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

            while (Generation < maxGenerations) //best fitness < maxfitness, otherwise stop
                NextGeneration();
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

            (Species s, Genome f) strongestSpecies = (null, null);
            foreach (Species s in Species)
            {
                s.UpdateData(Generation);
                s.CheckIfStagnant();

                Genome f = s.FittestGenome();
                if (strongestSpecies.s == null)
                    strongestSpecies = (s, f);
                else
                    strongestSpecies = (f > strongestSpecies.f) ? (s, f) : strongestSpecies;
            }
            strongestSpecies.s.CheckIfRefocus();
            //refocus is not yet complete! something is probably wrong with fitness func

            

        }
        /*
          


        */
    }
}
