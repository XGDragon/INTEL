using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Connection
    {
        private static Dictionary<(int from, int to), int> _connections = new Dictionary<(int from, int to), int>();
        private static int _innovation = 0;
        public static void Clear() { _connections.Clear(); _innovation = 0; }

        public int Innovation { get; private set; }
        public Node From { get; private set; }
        public Node To { get; private set; }
        public double Weight { get; private set; }
        public bool Enable { get; set; }
        
        public Connection(Node from, Node to)
        {
            int i;
            if (_connections.TryGetValue((from.ID, to.ID), out i))
                Innovation = i;
            else
            {
                Innovation = _innovation;
                _connections.Add((from.ID, to.ID), _innovation++);
            }

            From = from;
            To = to;
            Weight = (Program.R.NextDouble() * Parameter.MutationWeightRange) - (Parameter.MutationWeightRange / 2);
            Enable = true;
        }

        public Connection(Node from, Node to, Connection copy)
        {
            Innovation = copy.Innovation;
            From = from;
            To = to;
            Weight = copy.Weight;
            Enable = copy.Enable;
        }

        public Connection(Node from, Node to, Connection copy1, Connection copy2)
        {
            Innovation = copy1.Innovation;
            From = from;
            To = to;

            if (Program.R.NextDouble() < Parameter.CrossoverMultipoint)
                Weight = (copy1.Weight + copy2.Weight) / 2;
            else
            {
                Weight = (Program.R.NextDouble() < 0.5) ? copy1.Weight : copy2.Weight;
            }

            if (copy1.Enable && copy2.Enable)
                Enable = true;
            else
                Enable = (!copy1.Enable && !copy2.Enable) ? false : (Program.R.NextDouble() > Parameter.CrossoverDisable);
        }

        public void ReEnable()
        {
            if (!Enable)
                Enable = (Program.R.NextDouble() < Parameter.MutationReEnable);
        }

        public void MutateWeight()
        {
            if (Program.R.NextDouble() < Parameter.MutationChangeWeight)
                Weight =
                    Math.Max(-Parameter.MutationMaxWeightRange,
                        Math.Min(Parameter.MutationMaxWeightRange,
                            Weight + Parameter.MutationWeightRange * (Program.R.NextDouble() - 0.5d)));
        }

        public override string ToString()
        {
            string d = (Enable) ? "" : "DISABLED** ";
            return d + Innovation + ": [" + Weight.ToString("F") + "] " + From.ID + "(" + From.NodeType + ") > " + To.ID + "(" + To.NodeType + ")";
        }

        public static double operator +(Connection a, Connection b) { return a.Weight + b.Weight; }
        public static double operator -(Connection a, Connection b) { return a.Weight - b.Weight; }
    }
}
