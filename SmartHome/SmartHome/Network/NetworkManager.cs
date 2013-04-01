using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network
{
    public class NetworkManager
    {
        public Security Security { get; set; }
        public List<Node> Nodes { get; set; }

        public List<HomeDevice> HomeDevices {get; set;}
    }
}
