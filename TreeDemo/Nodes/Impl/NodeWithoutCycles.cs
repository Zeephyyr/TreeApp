using System;
using System.Collections.Generic;

namespace Nodes.Impl
{
    public class NodeWithoutCycles
    {
        public List<NodeWithoutCycles> Children { get; set; }

        public Guid ParentId { get; set; }

        public int DigitValue { get; set; }

        public string StringValue { get; set; }

        public int Generation { get; set; }

        public Guid Id { get; set; }
    }
}
