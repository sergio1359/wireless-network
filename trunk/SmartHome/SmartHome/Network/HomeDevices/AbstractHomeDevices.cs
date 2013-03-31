using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions.Enums;

namespace SmartHome.Network.HomeDevices
{
    public abstract class HomeDevice
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public Connector Connector { get; set; }
        public Enums.ConnectorType ConnectorType { get; set; }



        public void RefreshState()
        {

        }
    }
}
