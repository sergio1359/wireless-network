using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartHome.Plugins;
using SmartHome.Products;
using SmartHome.Tools;

namespace SmartHome.Network
{
    public class Node
    {
        public int ID { get; set; }
        public uint Mac { get; set; }
        public string Name { get; set; }
        public byte NetworkRetries = 3;
        public ushort Address { get; set; }
        public List<Connector> Connectors { get; set; }
        public Position Position { get; set; }
        public BaseType Base { get; set; }
        public ShieldType Shield { get; set; }
        public Security Security { get; set; }

        public Base GetBaseConfiguration()
        {
            return ProductConfiguration.GetBaseConfiguration(Base);
        }

        public TimeAction[] GetTimeActions()
        {
            return Sheduler.TimeActions.Where(x => x.ToHomeDevice.Connector.Node.Address == Address).ToArray();
        }
    }
}
