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

        public Problem Problem { get; private set; }

        public List<Genome> Population = new List<Genome>();
        public List<Species> Species = new List<Species>();
        //public List<Species> Species = new List<Species>(); //issues with who controls who

        public int Generation { get; private set; }

        public Algorithm() { R = new Random(); }

        public void Run(Problem p, int maxGenerations)
        {
            Problem = p;
            Initialize();
            while (Generation < maxGenerations)
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
                g.EvaluateFitness(Problem.FitnessFunc);
        }
/*
   for index_individual=2:size(population,2);
        assigned_existing_species_flag=0;
      new_species_flag=0;
      index_species=1;
      while assigned_existing_species_flag==0 & new_species_flag==0 %loops through the existing species, terminates when either the individual is assigned to existing species or there are no more species to test it against, which means it is a new species
         distance = speciation.c3 * sum(abs(population(index_individual).connectiongenes(4,:) - matrix_reference_individuals(index_species,:))) / number_connections; %computes compatibility distance, abbreviated, only average weight distance considered
         if distance<speciation.threshold %If within threshold, assign to the existing species
            population(index_individual).species=index_species;
            assigned_existing_species_flag=1;
            species_record(index_species).number_individuals=species_record(index_species).number_individuals+1;
         end
         index_species = index_species + 1;
         if index_species>size(matrix_reference_individuals,1) & assigned_existing_species_flag==0 %Outside of species references, must be new species
            new_species_flag = 1;
        end
     end
      if new_species_flag==1 %check for new species, if it is, update the species_record and use individual as reference for new species
         population(index_individual).species=index_species;
         matrix_reference_individuals=[matrix_reference_individuals;population(index_individual).connectiongenes(4,:)];
         species_record(index_species).ID=index_species;
         species_record(index_species).number_individuals=1; %if number individuals in a species is zero, that species is extinct

      end
   end
   */
    }
}
