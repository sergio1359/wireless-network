using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartHome.Plugins;
using SmartHome.Products;

namespace SmartHome.Network
{
    public class Node
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public UInt16 Address { get; set; }
        public List<Connector> Connectors { get; set; }
        public Position Position { get; set; }
        public BaseType Base { get; set; }
        public ShieldType Shield { get; set; }
        public Security Security { get; set; }

        public Base GetBaseConfiguration()
        {
            return ProductConfiguration.GetBaseConfiguration(Base);
        }
    }
}
