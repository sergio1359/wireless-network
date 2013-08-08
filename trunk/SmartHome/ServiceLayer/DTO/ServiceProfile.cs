#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmartHome.Network;
using SmartHome.Network.HomeDevices; 
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

        //TODO: Esto hay que revisarlo en la otra direccion y definir el DTO de posicion y operacion
        private void Map()
        {
            Mapper.CreateMap<Connector, ConnectorDTO>()
                .ForMember(t => t.ConnectorType, f => f.MapFrom(c => Enum.GetName(typeof(ConnectorType), c.ConnectorType)));

            Mapper.CreateMap<HomeDevice, HomeDeviceDTO>()
                .ForMember(t => t.Type, f => f.MapFrom(hd => hd.HomeDeviceType));

            Mapper.CreateMap<Node, NodeDTO>()
                .ForMember(t => t.Base, f => f.MapFrom(n => Enum.GetName(typeof(BaseType), n.Base)))
                .ForMember(t => t.Shield, f => f.MapFrom(n => Enum.GetName(typeof(ShieldType), n.Shield)));

            Mapper.CreateMap<Operation, OperationDTO>();

            Mapper.CreateMap<Position, PositionDTO>();
        }
    }
}
