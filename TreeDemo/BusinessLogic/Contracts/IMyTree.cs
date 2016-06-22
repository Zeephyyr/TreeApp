using DataTransfer;

namespace BusinessLogic.Contracts
{
    public interface IMyTree
    {
        void CleaniseTree(RemoveTreeRequestDto data);

        GetKeyValuesResponseDto GetKeyValues(GetKeyValuesRequestDto data);

        bool AddNode(AddNodeRequestDto data);

        void RemoveNode(RemoveNodeRequestDto data);

        OrderedValuesResponseDto OrderedValues(OrderedValuesRequestDto data);

        NodeResponseDto GetTreeFromDb(GetTreeRequestDto data);

        NodeResponseDto GetNode(NodeRequestDto data);

        bool UpdateNodeData(ChangeDataRequestDto data);

        void CreateTree(CreateTreeDto data);

        void StoreXml(StoreXmlRequestDto data);

        void RestoreFromXml(RestoreFromXmlRequestDto data);
    }
}
