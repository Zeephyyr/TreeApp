using System;

namespace DataTransfer
{
    public class AddNodeRequestDto
    {
        public Guid UserId { get; set; }

        public Guid NodeId { get; set; }

        public int DigitValue { get; set; }

        public string StringValue { get; set; }

        public Guid ParentId { get; set; }

        public int Generation { get; set; }
    }
}
