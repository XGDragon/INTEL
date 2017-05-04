using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    class Node
    {
        private const decimal BIAS_INPUT = 1;

        public enum Type { Input, Output, Hidden, Bias };

        public int ID { get; protected set; }
        public Type NodeType { get; protected set; }

        private decimal _input = 0;
        public virtual decimal Input { get { return (NodeType == Type.Bias) ? BIAS_INPUT : _input; } set { _input = value; } }
        public decimal Output { get; protected set; }

        private List<Connection> _connections = new List<Connection>();

        public Node(int id, Type type)
        {
            ID = id;
            NodeType = type;
        }

        public Node(Node copy)
        {
            ID = copy.ID;
            NodeType = copy.NodeType;            
        }

        public Connection Connect(Node target)
        {
            Connection c = new Connection(this, target);
            _connections.Add(c);
            return c;
        }

        public void Activation(Problem.ActivationFunction af)
        {
            Output = af(Input);
            Input = 0;
        }
        
        /// <summary>
        /// Each connected nodes Input gains this nodes Output * Weight of connection.
        /// </summary>
        public void Export()
        {
            foreach (Connection c in _connections)
                if (c.Enable)
                    c.To.Input += Output * c.Weight;
        }

        public override string ToString()
        {
            return ID + ": [" + NodeType + "] " + Input.ToString("F") + " > " + Output.ToString("F");
        }
    }
}
