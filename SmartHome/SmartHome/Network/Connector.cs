using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions.Enums;
using SmartHome.Network.HomeDevices;

namespace SmartHome.Network
{
    public class Connector
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Enums.ConnectorType ConnectorType { get; set; }
        public Node Node { get; set; }
        public HomeDevice HomeDevice { get; set; }
    }
}
