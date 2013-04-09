using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartHome.Plugins;

namespace SmartHome.Network
{
    public class Node
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public UInt16 Address { get; set; }
        public List<Connector> Connectors { get; set; }
        public Position Position { get; set; }
        public Plugins.BaseType Base { get; set; }
        public Plugins.ShieldType Shield { get; set; }
        public Security Security { get; set; }
    }
}
