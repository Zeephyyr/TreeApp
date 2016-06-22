using System;
using DataTransfer.Enums;

namespace DataTransfer
{
    public class OrderedValuesRequestDto
    {
        public Guid UserId { get; set; }

        public SortOrderBy OrderBy { get; set; }
    }
}
