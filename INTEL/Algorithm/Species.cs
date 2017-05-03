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
        public Genome Representative { get { return _members[0]; } }
        public int GenerationsExisted { get { return _data.Count; } }
        public bool EliminateSpecies { get; set; } //mean fitness to 0.01?
        public Data CurrentData { get; private set; }

        private List<Genome> _members = new List<Genome>();
        private Dictionary<int, Data> _data = new Dictionary<int, Data>();

        public Species(Genome representative)
        {
            Add(representative);
            EliminateSpecies = false;
        }

        public Genome FittestGenome()
        {
            return _members.Max();
        }

        public void UpdateData(int generation)
        {
            CurrentData = new Data(this, generation);
            _data[generation] = CurrentData;
        }

        public bool CheckIfStagnant()
        {
            if (Count > 0 && GenerationsExisted >= Parameter.StagnationGenerations)
            {
                var a = _data.Values;
                decimal avg = a.Average((Species.Data d) => { return d.MaxFitness; });
                return (a.Where((Species.Data d) => { return (Math.Abs(d.MaxFitness - avg) < Parameter.StagnationThreshold); }).Count() == Parameter.StagnationGenerations);
            }
            return false;
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
                g.MemberOf._members.Remove(g);

            g.MemberOf = this;
            g.MemberOf._members.Add(g);
        }

        public void Clear()
        {
            ((ICollection<Genome>)_members).Clear();
        }

        public bool Contains(Genome item)
        {
            return ((ICollection<Genome>)_members).Contains(item);
        }

        public void CopyTo(Genome[] array, int arrayIndex)
        {
            ((ICollection<Genome>)_members).CopyTo(array, arrayIndex);
        }

        public bool Remove(Genome g)
        {
            g.MemberOf = null;
            return ((ICollection<Genome>)_members).Remove(g);
        }

        public IEnumerator<Genome> GetEnumerator()
        {
            return ((ICollection<Genome>)_members).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<Genome>)_members).GetEnumerator();
        }

        public int Count => ((ICollection<Genome>)_members).Count;

        public bool IsReadOnly => ((ICollection<Genome>)_members).IsReadOnly;
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

                if (s._members.Count > 0)
                {
                    MeanFitness = s._members.Average((Genome g) => { return g.Fitness; });
                    Genome f = s._members.Max();
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
