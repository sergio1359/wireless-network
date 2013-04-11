using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartHome.Plugins;
using SmartHome.Network.HomeDevices;
using SmartHome.Products;

namespace SmartHome.Network
{
    public class Connector
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ConnectorType ConnectorType { get; set; }
        public Node Node { get; set; }
        public HomeDevice HomeDevice { get;
            set 
            {
                if (value == null)
                    HomeDevice.Connector = null;
                else
                    HomeDevice.Connector = this;
                HomeDevice = value;                
            }
        }

        public List<PinPort> GetPinPort()
        {
            return ProductConfiguration.GetShieldDictionary(Node.Shield)[Name];
        }

        public PinPortConfiguration GetPinPortConfiguration()
        {
            return ProductConfiguration.GetPinPortConfiguration(HomeDevice.HomeDeviceType);
        }
    }
}
