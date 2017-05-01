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
        private static Dictionary<(int from, int to), uint> _connections = new Dictionary<(int from, int to), uint>();
        private static uint _innovation = 0;
        public static void Clear() { _connections.Clear(); _innovation = 0; }

        public uint Innovation { get; private set; }
        public int From { get; private set; }
        public int To { get; private set; }
        public decimal Weight { get; private set; }
        public bool Enable { get; private set; }

        public Connection(int from, int to)
        {
            uint i;
            if (_connections.TryGetValue((from, to), out i))
                Innovation = i;
            else
            {
                Innovation = _innovation;
                _connections.Add((from, to), _innovation++);
            }

            From = from;
            To = to;            
            Weight = ((decimal)Algorithm.R.NextDouble() * Parameter.MutationInitWeightRange) - (Parameter.MutationInitWeightRange / 2);
            Enable = true;
        }

        public Connection(Node from, Node to)
        {
            uint i;
            if (_connections.TryGetValue((from.ID, to.ID), out i))
                Innovation = i;
            else
                _connections.Add((from.ID, to.ID), _innovation++);

            From = from.ID;
            To = to.ID;
            Weight = ((decimal)Algorithm.R.NextDouble() * Parameter.MutationInitWeightRange) - (Parameter.MutationInitWeightRange / 2);
            Enable = true;
        }

        public static decimal operator +(Connection a, Connection b) { return a.Weight + b.Weight; }
        public static decimal operator -(Connection a, Connection b) { return a.Weight - b.Weight; }
    }
}
