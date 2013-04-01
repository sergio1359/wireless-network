using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions.Enums;

namespace SmartHome.Network
{
    public class Node
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public UInt16 Address { get; set; }
        public List<Connector> Connectors { get; set; }
        public Position Position { get; set; }
        public Enums.BaseType Base { get; set; }
        public Enums.ShieldType Shield { get; set; }
        public Security Security { get; set; }
    }
}
