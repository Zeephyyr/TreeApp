using AutoMapper;
using DataTransfer;
using Nodes.Impl;
using Repositories;

namespace Mapping
{
    public class MapperHelper
    {
        private static MapperConfiguration _config;

        public void Initialize()
        {
            _config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RemoveTreeRequestDto, RemoveTreeRequestDao>();
                cfg.CreateMap<RemoveTreeRequestDao, RemoveTreeRequestDto>();
                cfg.CreateMap<CreateTreeDto, AddNodeRequestDao>();
                cfg.CreateMap<AddNodeRequestDao, CreateTreeDto>();
                cfg.CreateMap<Node, AddNodeRequestDao>();
                cfg.CreateMap<AddNodeRequestDao, Node>();
                cfg.CreateMap<GetNodeResponseDao, NodeResponseDto>();
                cfg.CreateMap<NodeResponseDto, GetNodeResponseDao>();
                cfg.CreateMap<Node, NodeResponseDto>();
                cfg.CreateMap<NodeResponseDto, Node>();
                cfg.CreateMap<NodeRequestDto, GetNodeRequestDao>();
                cfg.CreateMap<GetNodeRequestDao, NodeRequestDto>();
                cfg.CreateMap<ChangeDataRequestDto, UpdateNodeRequestDao>();
                cfg.CreateMap<UpdateNodeRequestDao, ChangeDataRequestDto>();
                cfg.CreateMap<GetNodeResponseDao, Node>();
                cfg.CreateMap<Node, GetNodeResponseDao>();
                cfg.CreateMap<NodeWithoutCycles, Node>();
                cfg.CreateMap<Node, NodeWithoutCycles>();
            });
        }
        

        public TDest GetValue<TSource, TDest>(TSource sourceObj)
        {
            IMapper mapper = _config.CreateMapper();
            return mapper.Map<TSource, TDest>(sourceObj);
        }
    }
}
