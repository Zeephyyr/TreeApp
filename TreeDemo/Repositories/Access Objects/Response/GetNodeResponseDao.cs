using System;

namespace Repositories
{
    public class GetNodeResponseDao
    {
        public Guid Id { get; set; }

        public int DigitValue { get; set; }

        public string StringValue { get; set; }

        public Guid? ParentId { get; set; }

        public int Generation { get; set; }

        public Guid UserId { get; set; }
    }
}
