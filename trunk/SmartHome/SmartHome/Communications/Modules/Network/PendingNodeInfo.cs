using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace SmartHome.Communications.Modules.Network
{
    public class PendingNodeInfo
    {
        public string MacAddress { get; set; }

        public byte ShieldType { get; set; }

        public byte BaseType { get; set; }

        public ushort TemporalAddress { get; set; }

        public byte[] TemporalAESKey { get; set; }
    }
}
