using System;
using System.Collections.Generic;

namespace Nodes.Impl
{
    public class Node
    {
        public List<Node> Children { get; set; }

        public Node Parent { get; set; }

        public int DigitValue { get; set; }

        public string StringValue { get; set; }

        public int Generation { get; set; }

        public Guid Id { get; set; }
    }
}
