using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Plugins;
using SmartHome.Comunications;
using SmartHome.Memory;
using System.Drawing;

namespace SmartHome.Network.HomeDevices
{
    public abstract class HomeDevice
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public Connector Connector { get; set; }
        public ConnectorType ConnectorCapable { get; set; }

        public bool InUse
        {
            get
            {
                return Connector != null;
            }
        }

        private static string[] homeDeviceTypes = null;
        public static string[] HomeDeviceTypes
        {
            get
            {
                if (homeDeviceTypes == null)
                    homeDeviceTypes = typeof(HomeDevice).Assembly.GetTypes().Where(t => t != typeof(HomeDevice).GetType() && typeof(HomeDevice).GetType().IsAssignableFrom(t)).Select(t => t.Name).ToArray();

                return homeDeviceTypes;
            }
        }

        public string HomeDeviceType
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public Position Position { get; set; }
        public List<Operation> Operations { get; set; }

        public HomeDevice() { }

        public HomeDevice(string name)
        {
            Name = name;
        }

        public void LinkConnector(Connector connector)
        {
            this.Connector = connector;
        }

        public void UnlinkConnector()
        {
            this.Connector = null;
        }

        public virtual void RefreshState()
        {

        }

    }
}
