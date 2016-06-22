using System;
using System.Collections.Generic;

namespace DataTransfer
{
    public class NodeResponseDto
    {
        public List<NodeResponseDto> Children { get; set; }

        public NodeResponseDto Parent { get; set; }

        public int DigitValue { get; set; }

        public string StringValue { get; set; }

        public int Generation { get; set; }

        public Guid Id { get; set; }

        public string WhiteSpace
        {
            get
            {
                string val = "";
                for (var i = 0; i < Generation; i++)
                {
                    val += "------";
                }
                return val;
            }
        }
    }
}
