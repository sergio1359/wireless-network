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
        public HomeDevice HomeDevice { get; set; }

        public bool InUse
        {
            get
            {
                if (HomeDevice != null)
                    return true;
                else
                    return false;
            }
        }

        public Connector() { }

        public Connector(string Name, ConnectorType type, Node node)
        {
            this.Name = Name;
            ConnectorType = type;
            Node = node;
        }

        public List<PinPort> GetPinPort()
        {
            return ProductConfiguration.GetShieldDictionary(Node.Shield)[Name].Item2;
        }

        public PinPortConfiguration GetPinPortConfiguration()
        {
            return ProductConfiguration.GetPinPortConfiguration(HomeDevice.HomeDeviceType);
        }

        public Operation[] GetActionsConnector()
        {
            if (HomeDevice == null)
            {
                return new Operation[0];
            }
            else
            {
                return HomeDevice.Operations.ToArray();
            }

        }
    }
}
