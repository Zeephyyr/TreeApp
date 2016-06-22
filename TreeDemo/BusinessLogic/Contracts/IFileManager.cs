using System;
using Nodes.Impl;

namespace BusinessLogic.Contracts
{
    public interface IFileManager
    {
        void SaveToFile(Tree tree, Guid userId);

        Tree RestoreFromFile(Guid userId);
    }
}
