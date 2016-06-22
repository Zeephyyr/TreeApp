using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BusinessLogic.Contracts;
using DataTransfer;
using DataTransfer.Enums;
using Mapping;
using Nodes.Impl;
using Repositories;
using Repositories.Contract;
using Repositories.Impl;

namespace BusinessLogic.Impl
{
    public class DefaultTree:IMyTree
    {
        private readonly SortMode _sortMode;
        private readonly SortOrderBy _orderByMode;
        private readonly IRepository _repository;
        private readonly MapperHelper _mapperHelper=new MapperHelper();
        private readonly IFileManager _fileManager;

        public DefaultTree()
        {
            _sortMode = (SortMode) int.Parse(ConfigurationManager.AppSettings["SortMode"]);
            _orderByMode = (SortOrderBy)int.Parse(ConfigurationManager.AppSettings["OrderBySortMode"]);
            var repoType = (RepoType) int.Parse(ConfigurationManager.AppSettings["RepoType"]);
            _fileManager = new XmlFileManager();

            switch (repoType)
            {
                case RepoType.AdoNet:
                    _repository=new AdoDbRepo();
                    break;
            }
        }

        #region publics

        public void CleaniseTree(RemoveTreeRequestDto data)
        {
            _repository.RemoveTree(_mapperHelper.GetValue<RemoveTreeRequestDto,RemoveTreeRequestDao>(data));
        }

        public GetKeyValuesResponseDto GetKeyValues(GetKeyValuesRequestDto data)
        {
            Node root = AssembleTree(data.UserId);
            NodeResponseDto response = _mapperHelper.GetValue<Node, NodeResponseDto>(root);

            GetKeyValuesResponseDto result = new GetKeyValuesResponseDto
            {
                Max = response,
                Min = response,
                Sum = 0
            };

            GetKeyValuesInternal(ref result, response);

            return result;
        }

        public bool AddNode(AddNodeRequestDto data)
        {
            CheckIfValueExistsRequestDao checkData = new CheckIfValueExistsRequestDao
            {
                DigitValue = data.DigitValue,
                UserId = data.UserId
            };

            if (_repository.CheckIfValueExists(checkData)==Guid.Empty)
            {
                Node root = AssembleTree(data.UserId);
                Node neededNode = new Node();
                GetNodeById(data.ParentId, root, ref neededNode);

                AddNodeRequestDao nodeToAdd = new AddNodeRequestDao
                {
                    DigitValue = data.DigitValue,
                    Generation = neededNode.Generation + 1,
                    UserId = data.UserId,
                    StringValue = data.StringValue,
                    Id = Guid.NewGuid(),
                    ParentId = neededNode.Id
                };
                _repository.AddNode(nodeToAdd);
                return true;
            }
            return false;
        }

        public void CreateTree(CreateTreeDto data)
        {
            _repository.AddNode(_mapperHelper.GetValue<CreateTreeDto,AddNodeRequestDao>(data));
        }

        public void RemoveNode(RemoveNodeRequestDto data)
        {
            Node root = AssembleTree(data.UserId);

            Node neededNode = new Node();
            GetNodeById(data.NodeId, root, ref neededNode);

            if (neededNode == root)
            {
                CleaniseTree(new RemoveTreeRequestDto {UserId = data.UserId});
            }
            else
            {
                Node parent = new Node();
                FindParent(data.NodeId, root, ref parent);
                parent.Children.Remove(neededNode);

                if (neededNode.Children != null && neededNode.Children.Count > 0)
                {
                    RelinkChildrenForNode(neededNode);
                }

                List<Node> collection = new List<Node>();
                DisassembleTree(root, collection);

                _repository.RemoveTree(new RemoveTreeRequestDao
                {
                    UserId = data.UserId
                });
                var storeData = StoreTreeRequestDao(data.UserId, collection);

                _repository.StoreTree(storeData);
            }
        }

        public OrderedValuesResponseDto OrderedValues(OrderedValuesRequestDto data)
        {
            GetTreeResponseDao nodesFromDb = _repository.GetTree(new GetTreeRequestDao
            {
                UserId = data.UserId
            });
            List<NodeResponseDto> nodes = _mapperHelper.GetValue<List<GetNodeResponseDao>,List<NodeResponseDto>>(nodesFromDb.Nodes);

            if (data.OrderBy == SortOrderBy.Undefined)
            {
                data.OrderBy = _orderByMode;
            }

            switch (_sortMode)
            {
                case SortMode.Comparer:
                    switch (data.OrderBy)
                    {
                        case SortOrderBy.Asc:
                            nodes.Sort(new AscComparer());
                            break;
                        case SortOrderBy.Desc:
                            nodes.Sort(new DescComparer());
                            break;
                    }
                    break;
                case SortMode.Manual:
                    nodes = SimpleBubbleSort(nodes, data.OrderBy);
                    break;
            }
            return new OrderedValuesResponseDto
            {
                Nodes = nodes
            };
        }

        public NodeResponseDto GetTreeFromDb(GetTreeRequestDto data)
        {
            Node tree = AssembleTree(data.UserId);
            if (tree == null)
            {
                return null;
            }
            return _mapperHelper.GetValue<Node, NodeResponseDto>(tree);
        }

        public NodeResponseDto GetNode(NodeRequestDto data)
        {
            Node root = AssembleTree(data.UserId);
            Node neededNode = new Node();
            GetNodeById(data.NodeId, root, ref neededNode);
            return _mapperHelper.GetValue<Node, NodeResponseDto>(neededNode);
        }

        public bool UpdateNodeData(ChangeDataRequestDto data)
        {
            CheckIfValueExistsRequestDao checkData = new CheckIfValueExistsRequestDao
            {
                DigitValue = data.DigitValue,
                UserId = data.UserId
            };

            var nodeId = _repository.CheckIfValueExists(checkData);
            if (nodeId == Guid.Empty|| nodeId == data.NodeId)
            {
                UpdateNodeRequestDao updateData =
                    _mapperHelper.GetValue<ChangeDataRequestDto, UpdateNodeRequestDao>(data);
                _repository.UpdateNode(updateData);
                return true;
            }
            return false;
        }

        public void StoreXml(StoreXmlRequestDto data)
        {
            Node root = AssembleTree(data.UserId);
            Tree tree=new Tree
            {
                Root = _mapperHelper.GetValue<Node,NodeWithoutCycles>(root)
            };

            ConvertLinks(root, tree.Root);
            _fileManager.SaveToFile(tree,data.UserId);
        }

        public void RestoreFromXml(RestoreFromXmlRequestDto data)
        {
            Tree tree = _fileManager.RestoreFromFile(data.UserId);
            NodeWithoutCycles root = tree.Root;
            Node treeWithLinks = _mapperHelper.GetValue<NodeWithoutCycles, Node>(root);

            if (treeWithLinks.Children != null && treeWithLinks.Children.Count > 0)
            {
                foreach (var child in treeWithLinks.Children)
                {
                    AddLinks(treeWithLinks, child);
                }
            }

            List<Node> result=new List<Node>();
            DisassembleTree(treeWithLinks, result);

            var storeData = StoreTreeRequestDao(data.UserId, result);

            _repository.RemoveTree(new RemoveTreeRequestDao
            {
                UserId = data.UserId
            });
            _repository.StoreTree(storeData);
        }

        #endregion

        #region privates

        private void AddLinks(Node parent,Node child)
        {
            child.Parent = parent;
            if (child.Children != null && child.Children.Count > 0)
            {
                foreach (var node in child.Children)
                {
                    AddLinks(child,node);
                }
            }
        }

        private void ConvertLinks(Node root, NodeWithoutCycles anotherRoot)
        {
            if (root.Parent != null)
            {
                anotherRoot.ParentId = root.Parent.Id;
            }
            if (root.Children != null && root.Children.Count > 0)
            {
                foreach (var child in root.Children)
                {
                    ConvertLinks(child, anotherRoot.Children.First(x => x.Id == child.Id));
                }
            }
        }

        private StoreTreeRequestDao StoreTreeRequestDao(Guid userId, List<Node> result)
        {
            var storeData = new StoreTreeRequestDao
            {
                Tree = _mapperHelper.GetValue<List<Node>, List<AddNodeRequestDao>>(result)
            };
            foreach (var node in storeData.Tree)
            {
                var element = result.First(x => x.Id == node.Id);
                if (element.Parent != null)
                    node.ParentId = element.Parent.Id;
                else
                    node.ParentId = null;
                node.UserId = userId;
            }
            return storeData;
        }

        private List<NodeResponseDto> SimpleBubbleSort(List<NodeResponseDto> nodes,SortOrderBy sortOrderBy)
        {
            for (int i = 1; i < nodes.Count; i++)
            {
                for (int j = 1; j < nodes.Count; j++)
                {
                    switch (sortOrderBy)
                    {
                        case SortOrderBy.Asc:
                            if (nodes[j].DigitValue < nodes[j - 1].DigitValue)
                            {
                                SwapObjects(nodes, j);
                            }
                            break;
                        case SortOrderBy.Desc:
                            if (nodes[j].DigitValue > nodes[j - 1].DigitValue)
                            {
                                SwapObjects(nodes, j);
                            }
                            break;
                    }
                }
            }
            return nodes;
        }

        private void SwapObjects(List<NodeResponseDto> nodes, int j)
        {
            NodeResponseDto tempObj = nodes[j];
            nodes[j] = nodes[j - 1];
            nodes[j - 1] = tempObj;
        }

        private Node AssembleTree(Guid id)
        {
            GetTreeResponseDao nodesFromDb = _repository.GetTree(new GetTreeRequestDao
            {
                UserId = id
            });

            if (nodesFromDb.Nodes == null || nodesFromDb.Nodes.Count == 0)
            {
                return null;
            }

            GetNodeResponseDao rootFromDb = nodesFromDb.Nodes.First(x => x.Generation == 0);
            Node root = _mapperHelper.GetValue<GetNodeResponseDao, Node>(rootFromDb);
            Node trueRoot = root;
            GetTreeRecursive(root, nodesFromDb.Nodes);

            return trueRoot;
        }

        private void GetTreeRecursive(Node root, List<GetNodeResponseDao> nodesFromDb)
        {
            if (nodesFromDb.Any(x => x.ParentId == root.Id))
            {
                root.Children = new List<Node>();
                nodesFromDb.ForEach(x =>
                {
                    if (x.ParentId == root.Id)
                    {
                        Node temp = _mapperHelper.GetValue<GetNodeResponseDao, Node>(x);
                        root.Children.Add(temp);
                    }
                });
            }
            if (root.Children != null && root.Children.Count > 0)
            {
                foreach (var child in root.Children)
                {
                    child.Parent = root;
                    GetTreeRecursive(child, nodesFromDb);
                }
            }
        }

        private void GetNodeById(Guid id,Node root,ref Node searchedNode)
        {
            if (root.Id == id)
            {
                searchedNode=root;
            }
            if (root.Children != null && root.Children.Count > 0)
            {
                foreach (var child in root.Children)
                {
                    GetNodeById(id, child, ref searchedNode);
                }
            }
        }

        private void RelinkChildrenForNode(Node node)
        {
            foreach (var child in node.Children)
            {
                child.Parent = node.Parent;
                child.Generation -= 1;
                node.Parent.Children.Add(child);
            }
        }

        private void GetKeyValuesInternal(ref GetKeyValuesResponseDto currentResult, NodeResponseDto root)
        {
            if (root.DigitValue < currentResult.Min.DigitValue)
            {
                currentResult.Min = root;
            }
            if (root.DigitValue > currentResult.Max.DigitValue)
            {
                currentResult.Max = root;
            }
            currentResult.Sum += root.DigitValue;

            if (root.Children != null && root.Children.Count > 0)
            {
                foreach (var child in root.Children)
                {
                    GetKeyValuesInternal(ref currentResult, child);
                }
            }
        }

        private void DisassembleTree(Node root, List<Node> result)
        {
            result.Add(root);
            if (root.Children != null&&root.Children.Count>0)
            {
                foreach (var child in root.Children)
                {
                    DisassembleTree(child, result);
                }
            }
        }

        private void FindParent(Guid nodeId, Node root, ref Node parentNode)
        {
            if (root.Children != null && root.Children.Count > 0)
            {
                if (root.Children.Any(x => x.Id == nodeId))
                {
                    parentNode = root;
                }
                foreach (var child in root.Children)
                {
                    FindParent(nodeId, child, ref parentNode);
                }
            }
        }

        #endregion
    }
}
