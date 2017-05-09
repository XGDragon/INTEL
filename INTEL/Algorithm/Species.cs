using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace INTEL
{
    class Species : IComparable<Species>
    {
        public Genome Representative { get; private set; }
        public int GenerationsExisted { get { return _snapshots.Count; } }
        public Snapshot CurrentSnapshot { get; private set; }
        public int AllowedOffspring { get; private set; }
        public bool Elite { get { return (AllowedOffspring > 0 && _parents.Count > Parameter.ElitismThreshold); } }
        public int ParentCount { get { return _parents.Count; } }

        private List<Genome> _offspring = new List<Genome>();   //before fitness evaluations
        private List<Genome> _parents = new List<Genome>();     //after fitness evaluations

        private List<Snapshot> _snapshots = new List<Snapshot>();    

        public Species(Genome representative)
        {
            AddOffspring(representative);
            Representative = representative;
        }

        public void EvaluateSpecies(Problem[] problems)
        {
            _parents.Clear();

            foreach (Genome o in _offspring)
            {
                o.EvaluateFitness(problems);
                _parents.Add(o);
            }

            Representative = RandomParent();
            _offspring.Clear();

            CurrentSnapshot = new Snapshot(this);
            _snapshots.Add(CurrentSnapshot);
        }

        public Genome FittestGenome()
        {
            return _parents.Max();
        }

        public Genome RandomParent()
        {
            return (_parents.Count > 0) ? _parents[Program.R.Next(_parents.Count)] : null;
        }

        public void CheckIfStagnant()
        {
            if (_parents.Count > 0 && GenerationsExisted >= Parameter.StagnationGenerations)
            {
                var a = _snapshots;
                double avg = a.Average((Species.Snapshot d) => { return d.MaxFitness; });
                if (a.TrueForAll(d => { return Math.Abs(d.MaxFitness - avg) < Parameter.StagnationThreshold; }))
                    EliminateSpecies();
            }
        }

        public bool CheckIfRefocus()
        {
            if (GenerationsExisted >= Parameter.RefocusGenerations)
            {
                var slice = _snapshots.GetRange(GenerationsExisted - (int)Parameter.RefocusGenerations, (int)Parameter.RefocusGenerations);
                double avg = slice.Average((Species.Snapshot d) => { return d.MaxFitness; });
                return slice.TrueForAll(d => { return Math.Abs(d.MaxFitness - avg) < Parameter.RefocusThreshold; });
            }
            return false;
        }

        public void EliminateSpecies()
        {
            CurrentSnapshot.FalsifyMeanFitness();
        }

        public void CalculateOffspring(ref double overflow, double globalMeanFitness)
        {
            double number_offspring = CurrentSnapshot.MeanFitness / globalMeanFitness * Parameter.PopulationSize;
            overflow += number_offspring - Math.Truncate(number_offspring);
            if (overflow >= 1)
            {
                AllowedOffspring = (int)Math.Ceiling(number_offspring);
                overflow--;
            }
            else
                AllowedOffspring = (int)Math.Floor(number_offspring);
        }

        public void CullTheWeak()
        {
            if (_parents.Count > Parameter.KillAmount && Math.Ceiling(_parents.Count * (1 - Parameter.KillPercentage)) > 2)
            {
                _parents.Sort();    //worst to best
                _parents.RemoveRange(0, (int)Math.Floor(_parents.Count * Parameter.KillPercentage));
            }
        }

        public Genome[] SelectParents(int crossoverCount, int mutationCount)
        {
            Genome[] selected = new Genome[2 * crossoverCount + mutationCount];

            //use pressure to balloon fitness values for selection
            double[] pressuredFitness = new double[_parents.Count];
            for (int i = 0; i < pressuredFitness.Length; i++)
            {
                pressuredFitness[i] = Math.Pow(_parents[i].Fitness, Parameter.SelectionPressure);
                if (i > 0)
                    pressuredFitness[i] += pressuredFitness[i - 1];
            }
            double sum = pressuredFitness.Last();
            for (int i = 0; i < pressuredFitness.Length; i++)
                pressuredFitness[i] /= sum;

            //stochastic universal sampling
            int n = selected.Length;
            double pointStart = Program.R.NextDouble() / n;
            double d = pointStart;
            for (int i = 0, p = 0; i < n; i++, d += pointStart) //loop pointers
            {
                while (d > pressuredFitness[p])
                    p++;
                selected[i] = _parents[p];                
            }

            //fisheryates shuffle
            while (n > 1)
            {
                int k = Program.R.Next(n--);
                var temp = selected[n];
                selected[n] = selected[k];
                selected[k] = temp;
            }

            return selected;
        }

        public int CompareTo(Species other)
        {
            if (this.CurrentSnapshot.MaxFitness > other.CurrentSnapshot.MaxFitness)
                return 1;
            if (this.CurrentSnapshot.MaxFitness < other.CurrentSnapshot.MaxFitness)
                return -1;
            else return 0;
        }
        
        public void AddOffspring(Genome g)
        {
            if (g.MemberOf != null)
                g.MemberOf._offspring.Remove(g);

            g.MemberOf = this;
            g.MemberOf._offspring.Add(g);
        }

        public override string ToString()
        {
            return _offspring.Count + "(+"+AllowedOffspring+") offspring, " + _parents.Count + " parents; " + CurrentSnapshot.ToString();
        }

        public struct Snapshot : IComparable<Snapshot>
        {
            private const double ELIMINATION_FITNESS = 0.1d;

            public double MeanFitness { get { return (EliminateSpecies) ? ELIMINATION_FITNESS : _meanFitness; } }
            public double MaxFitness { get; private set; }
            public Genome FittestGenome { get; private set; }
            public bool EliminateSpecies { get; private set; }

            private double _meanFitness;

            public Snapshot(Species s)
            {
                EliminateSpecies = false;
                if (s._parents.Count > 0)
                {
                    _meanFitness = s._parents.Average((Genome g) => { return g.Fitness; });
                    Genome f = s._parents.Max();
                    MaxFitness = f.Fitness;
                    FittestGenome = f;
                }
                else
                {
                    _meanFitness = 0;
                    MaxFitness = 0;
                    FittestGenome = null;
                }
            }

            public void FalsifyMeanFitness()
            {
                EliminateSpecies = true;
            }

            public int CompareTo(Snapshot other)
            {
                if (this.MaxFitness > other.MaxFitness)
                    return 1;
                if (this.MaxFitness < other.MaxFitness)
                    return -1;
                else return 0;
            }

            public override string ToString()
            {
                return "Mean=" + MeanFitness.ToString("F") + ", Max=" + MaxFitness.ToString("F");
            }
        }
    }
}
