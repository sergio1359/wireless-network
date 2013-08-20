#region Using Statements
using AutoMapper;
using DataLayer.Entities;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System;
using SmartHome.Communications.Modules.Network;
#endregion

namespace ServiceLayer.DTO
{
    public class ServiceProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ServiceProfile"; }
        }

        protected override void Configure()
        {
            Map();
        }

        private void Map()
        {
            Mapper.CreateMap<Connector, ConnectorDTO>()
                .ForMember(t => t.ConnectorType, f => f.MapFrom(c => Enum.GetName(typeof(ConnectorTypes), c.ConnectorType)));

            Mapper.CreateMap<HomeDevice, HomeDeviceDTO>()
                .ForMember(t => t.Type, f => f.MapFrom(hd => hd.HomeDeviceType));

            Mapper.CreateMap<Location, LocationDTO>();

            Mapper.CreateMap<Log, LogDTO>();

            Mapper.CreateMap<Node, NodeDTO>()
                .ForMember(t => t.Base, f => f.MapFrom(n => Enum.GetName(typeof(BaseTypes), n.Base)))
                .ForMember(t => t.Shield, f => f.MapFrom(n => Enum.GetName(typeof(ShieldTypes), n.Shield)));

            Mapper.CreateMap<Operation, OperationDTO>()
                .ForMember(t => t.NameOperation, f => f.MapFrom(n => n.OperationName));

            Mapper.CreateMap<PendingNodeInfo, PendingNodeInfoDTO>()
                .ForMember(t => t.BaseType, f => f.MapFrom(n => (BaseTypes)n.BaseType))
                .ForMember(t => t.ShieldType, f => f.MapFrom(n => (ShieldTypes)n.ShieldType));

            Mapper.CreateMap<Theme, ThemeDTO>();

            Mapper.CreateMap<Zone, ZoneDTO>()
                .ForMember(t => t.Name, f => f.MapFrom(z => z.Name));

            Mapper.CreateMap<View, ViewDTO>();

            Mapper.CreateMap<TimeOperation, TimeOperationDTO>();
        }
    }
}
