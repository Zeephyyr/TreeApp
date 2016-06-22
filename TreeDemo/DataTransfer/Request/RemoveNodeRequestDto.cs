using System;

namespace DataTransfer
{
    public class RemoveNodeRequestDto
    {
        public Guid NodeId { get; set; }

        public Guid UserId { get; set; }
    }
}
