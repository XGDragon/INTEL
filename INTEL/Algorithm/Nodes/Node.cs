using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTEL
{
    abstract class Node
    {
        public int ID { get; protected set; }
        public enum Type { Input, Output, Hidden, Bias};
        public Type NodeType { get; protected set; }

        public decimal State { get; protected set; }
        public decimal Output { get; protected set; }

        public Node(int id)
        {
            ID = id;
        }
    }
}
