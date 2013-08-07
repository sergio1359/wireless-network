#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmartHome.Network; 
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
                // t: Destination entity (ConnectorDTO).
                // f: Source entity (Connector).
                // r: Source entity (Connector).
                //.ForMember(t => t.Name, f => f.MapFrom(r => r.Name))
                .ReverseMap();
        }
    }
}
