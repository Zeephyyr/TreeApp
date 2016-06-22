using System.Collections.Generic;
using System.Data.Entity;
using Nodes.Impl;

namespace Repositories.EntityFramework
{
    public class NodeContext:DbContext
    {
        public NodeContext() : base("MyConnection")
        {
           
        }

        public DbSet<GetTreeResponseDao> TreeResponse { get; set; }

        public DbSet<GetNodeResponseDao> NodeResponse { get; set; }

        public DbSet<AddNodeRequestDao> Nodes { get; set; }
    }
}
