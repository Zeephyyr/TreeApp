using System;

namespace DataTransfer
{
    public class ChangeDataRequestDto
    {
        public Guid NodeId { get; set; }

        public int DigitValue { get; set; }

        public string StringValue { get; set; }

        public Guid UserId { get; set; }

    }
}
