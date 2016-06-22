using System;

namespace DataTransfer
{
    public class CreateTreeDto
    {
        public Guid UserId { get; set; }

        public int DigitValue { get; set; }

        public string StringValue { get; set; }

        public Guid Id { get; set; }

        public int Generation => 0;

        public Guid? ParentId => null;
    }
}
