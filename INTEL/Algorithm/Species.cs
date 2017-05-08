using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Species : ICollection<Genome>, IComparable<Species>
    {
        public Genome Representative { get; private set; }
        public int GenerationsExisted { get { return _data.Count; } }
        public bool EliminateSpecies { get; set; } //mean fitness to 0.01?
        public Data CurrentData { get; private set; }
        public int Offspring { get; private set; }
        public bool Elite { get; set; }

        private List<Genome> _offspring = new List<Genome>();
        private List<Genome> _parents = new List<Genome>();
        private Dictionary<int, Data> _data = new Dictionary<int, Data>();        

        public Species(Genome representative)
        {
            Add(representative);
            Representative = representative;
            EliminateSpecies = false;
        }

        public void MakeMembersSenior()
        {
            _parents = _offspring;
            Representative = _parents[Program.R.Next(_parents.Count)];
        }

        public Genome FittestGenome()
        {
            return _parents.Max();
        }

        public Genome RandomParent()
        {
            return _parents[Program.R.Next(_parents.Count)];
        }

        public void UpdateData(int generation)
        {
            CurrentData = new Data(this, generation);
            _data[generation] = CurrentData;
        }

        public void CheckIfStagnant()
        {
            EliminateSpecies = false;
            if (Count > 0 && GenerationsExisted >= Parameter.StagnationGenerations)
            {
                var a = _data.Values;
                decimal avg = a.Average((Species.Data d) => { return d.MaxFitness; });
                EliminateSpecies = (a.Where((Species.Data d) => { return (Math.Abs(d.MaxFitness - avg) < Parameter.StagnationThreshold); }).Count() == Parameter.StagnationGenerations);
            }
        }

        public bool CheckIfRefocus()
        {
            if (GenerationsExisted >= Parameter.RefocusGenerations)
            {
                List<Species.Data> slice = new List<Data>();
                for (int i = GenerationsExisted - (int)Parameter.RefocusGenerations; i < GenerationsExisted; i++)
                    if (_data.ContainsKey(i))
                        slice.Add(_data[i]);
                decimal avg = slice.Average((Species.Data d) => { return d.MaxFitness; });
                return (slice.Where((Species.Data d) => { return (Math.Abs(d.MaxFitness - avg) < Parameter.RefocusThreshold); }).Count() == Parameter.RefocusGenerations);
            }
            return false;
        }

        public void CalculateOffspring(ref decimal overflow, decimal globalMeanFitness)
        {
            decimal number_offspring = CurrentData.MeanFitness / globalMeanFitness * Parameter.PopulationSize;
            overflow += number_offspring - Math.Truncate(number_offspring);
            if (overflow >= 1)
            {
                Offspring = (int)Math.Ceiling(number_offspring);
                overflow--;
            }
            else
                Offspring = (int)Math.Floor(number_offspring);
        }

        public void CullTheWeak()
        {
            _offspring.Sort();
            _offspring.RemoveRange(0, (int)Math.Floor(_offspring.Count * Parameter.KillPercentage));
        }

        public Genome[] Selection(int crossoverCount, int mutationCount)
        {
            Genome[] selected = new Genome[2 * crossoverCount + mutationCount];

            //fully random selection lol pls edit'. ps same selection is allowed
            for (int i = 0; i < selected.Length; i++)
                selected[i] = _offspring[Program.R.Next(_offspring.Count)];

            return selected;
        }

        public int CompareTo(Species other)
        {
            if (this.CurrentData.MaxFitness > other.CurrentData.MaxFitness)
                return 1;
            if (this.CurrentData.MaxFitness < other.CurrentData.MaxFitness)
                return -1;
            else return 0;
        }

        #region ICollection implementation
        public void Add(Genome g)
        {
            if (g.MemberOf != null)
                g.MemberOf._offspring.Remove(g);

            g.MemberOf = this;
            g.MemberOf._offspring.Add(g);
        }

        public void Clear()
        {
            ((ICollection<Genome>)_offspring).Clear();
        }

        public bool Contains(Genome item)
        {
            return ((ICollection<Genome>)_offspring).Contains(item);
        }

        public void CopyTo(Genome[] array, int arrayIndex)
        {
            ((ICollection<Genome>)_offspring).CopyTo(array, arrayIndex);
        }

        public bool Remove(Genome g)
        {
            g.MemberOf = null;
            return ((ICollection<Genome>)_offspring).Remove(g);
        }

        public IEnumerator<Genome> GetEnumerator()
        {
            return ((ICollection<Genome>)_offspring).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<Genome>)_offspring).GetEnumerator();
        }

        public int Count => ((ICollection<Genome>)_offspring).Count;

        public bool IsReadOnly => ((ICollection<Genome>)_offspring).IsReadOnly;
        #endregion
        
        public struct Data : IComparable<Data>
        {
            public int Generation { get; private set; }
            public decimal MeanFitness { get; private set; }
            public decimal MaxFitness { get; private set; }
            public Genome FittestGenome { get; private set; }

            public Data(Species s, int generation)
            {
                Generation = generation;

                if (s._offspring.Count > 0)
                {
                    MeanFitness = s._offspring.Average((Genome g) => { return g.Fitness; });
                    Genome f = s._offspring.Max();
                    MaxFitness = f.Fitness;
                    FittestGenome = f;
                }
                else
                {
                    MeanFitness = 0;
                    MaxFitness = 0;
                    FittestGenome = null;
                }
            }

            public int CompareTo(Data other)
            {
                if (this.MaxFitness > other.MaxFitness)
                    return 1;
                if (this.MaxFitness < other.MaxFitness)
                    return -1;
                else return 0;
            }

            public override string ToString()
            {
                return Generation + ": Mean=" + MeanFitness.ToString("F") + ", Max=" + MaxFitness.ToString("F");
            }
        }
    }
}
