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
        public decimal MaxFitness { get; private set; }

        public List<Genome> Population = new List<Genome>();
        public List<Species> Species = new List<Species>();
        //public List<Species> Species = new List<Species>(); //issues with who controls who
        
        public int Generation { get; private set; }

        public Algorithm(ProblemFactory pf)
        {
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
                    GenomeComparison gc = new GenomeComparison(Population[i], Species[index_species].Representative);
                    if (gc.Distance < Parameter.SpeciationThreshold)
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
            //Evaluate
            foreach (Genome g in Population)
                g.EvaluateFitness(Problem);
            
            //Check for Stagnation/Refocusing
            foreach (Species s in Species)
            {
                s.UpdateData(Generation);
                s.CheckIfStagnant();
            }

            if (Species.Max().CheckIfRefocus())
            {
                Species.Sort();     //from worst to best
                for (int i = 0; i < Species.Count - 2; i++) //exclude the 2 best species
                    Species[i].EliminateSpecies = (Species[i].CurrentData.MaxFitness > 0);
            }

            //check if solution found?

            List<Genome> newPopulation = new List<Genome>();
            Species.RemoveAll((Species s) => { return s.Count == 0; }); //remove empty species

            //Get offspring counts
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
            }
            
            //Crossover & mutation
            foreach (Species s in Species)
            {
                int overall = s.Offspring - ((s.Elite) ? 1 : 0);
                int crossovers = (int)Math.Round(overall * Parameter.CrossoverPercentage);
                int mutations = overall - crossovers;
                Genome[] selected = s.Selection(crossovers, mutations);
                int parent = 0;

                while (crossovers-- > 0)
                {
                    if (Program.R.NextDecimal() < Parameter.CrossoverInterspecies && Species.Count > 1)
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
                    ;
                    //get random ref from old pop if count > 0
            }
                //Speciation
                bool assigned = false;
                int popref = -1;
                while (!assigned && ++popref < newPopulation.Count)//i think
                {

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
