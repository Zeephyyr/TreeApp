namespace Repositories.Contract
{
    public interface IRepository
    {
        void StoreTree(StoreTreeRequestDao data);

        GetTreeResponseDao GetTree(GetTreeRequestDao data);

        void AddNode(AddNodeRequestDao addNode);

        void RemoveNode(RemoveNodeRequestDao data);

        void UpdateNode(UpdateNodeRequestDao data);

        bool CheckIfValueExists(CheckIfValueExistsRequestDao data);

        void RemoveTree(RemoveTreeRequestDao data);

        GetNodeResponseDao GetNode(GetNodeRequestDao data);
    }
}
