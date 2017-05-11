using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL.Network
{
    public class Node
    {
        private const double BIAS_INPUT = 1;

        public enum Type { Input, Output, Hidden, Bias };

        public int ID { get; protected set; }
        public Type NodeType { get; protected set; }

        private double _input = 0;
        public virtual double Input { get { return (NodeType == Type.Bias) ? BIAS_INPUT : _input; } set { _input = value; } }
        public double Output { get; protected set; }
        public double Depth { get; set; }
        
        //outgoing connections
        private Dictionary<Node, Connection> _connections = new Dictionary<Node, Connection>();
        
        public Node(int id, Type type) 
        {
            ID = id;
            NodeType = type;
            Depth = (type == Type.Output) ? 1 : 0;
        }

        public Node(Node copy) : this(copy.ID, copy.NodeType)
        {
            Depth = copy.Depth;
        }
        
        public bool HasConnections()
        {
            return _connections.Count > 0;
        }

        public bool ConnectsTo(Node target)
        {
            return _connections.ContainsKey(target);
        }

        public bool IsRecurrent(Node target)
        {
            return (target == this);
            //too complicated for now..
        }

        public void AddConnection(Connection c)
        {
            _connections[c.To] = c;
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
            foreach (Connection c in _connections.Values)
                if (c.Enable)
                    c.To.Input += Output * c.Weight;
        }

        public override string ToString()
        {
            return ID + ": [" + NodeType + "] " + Input.ToString("F") + " > " + Output.ToString("F");
        }
    }
}
