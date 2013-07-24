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
        public int Id { get; set; }
        public string Name { get; set; }
        public ConnectorType ConnectorType { get; set; }
        public Node Node { get; set; }
        public Dictionary<HomeDevice, List<PinPort>> MappingHomeDevice;

        public List<HomeDevice> HomeDevices
        {
            get
            {
                return MappingHomeDevice.Keys.ToList();
            }
        }

        public List<PinPort> PinPorts
        {
            get
            {
                return MappingHomeDevice.Values.SelectMany(v => v).ToList(); //TODO: quitar los repetidos
            }
        }

        public bool InUse
        {
            get
            {
                if (HomeDevices != null)
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

        public PinPortConfiguration GetPinPortConfiguration(HomeDevice homeDevice)
        {
            return ProductConfiguration.GetPinPortConfiguration(homeDevice);
        }

        public Operation[] GetActionsConnector()
        {
            if (!InUse)
            {
                return new Operation[0];
            }
            else
            {
                return HomeDevices.SelectMany(hd => hd.Operations).ToArray();
            }

        }
    }
}
