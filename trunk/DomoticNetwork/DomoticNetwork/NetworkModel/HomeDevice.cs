using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomoticNetwork;

namespace DomoticNetwork.NetworkModel
{
    class HomeDevice
    {
        public String Name { set; get; }
        public Enums.HomeDeviceType Type { set; get; }
        public Enums.ConectorType Connector { set; get; }
    }
}
