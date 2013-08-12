#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using DataLayer.Entities.Enums;
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

            Mapper.CreateMap<Node, NodeDTO>()
                .ForMember(t => t.Base, f => f.MapFrom(n => Enum.GetName(typeof(BaseTypes), n.Base)))
                .ForMember(t => t.Shield, f => f.MapFrom(n => Enum.GetName(typeof(ShieldTypes), n.Shield)));

            Mapper.CreateMap<Operation, OperationDTO>();

            Mapper.CreateMap<Zone, PlaceDTO>()
                .ForMember(t => t.Name, f => f.MapFrom(z => z.NameZone));


            Mapper.CreateMap<View, PlaceDTO>()
                .ForMember(t => t.Name, f => f.MapFrom(z => z.NameView));;

            Mapper.CreateMap<Location, LocationDTO>();
        }
    }
}
