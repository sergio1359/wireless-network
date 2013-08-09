using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Plugins;
using SmartHome.Comunications;
using SmartHome.Memory;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHome.Network.HomeDevices
{
    public abstract class HomeDevice
    {
        [Key]
        public ushort Id { get; set; }

        public string Name { get; set; }

        public Connector Connector { get; set; }

        public ConnectorType ConnectorCapable { get; set; }

        public Position Position { get; set; }

        public List<Operation> Operations { get; set; }

        [NotMapped]
        private static string[] homeDeviceTypes = null;
        public static string[] HomeDeviceTypes
        {
            get
            {
                if (homeDeviceTypes == null)
                    homeDeviceTypes = typeof(HomeDevice).Assembly.DefinedTypes.Where(t => t != typeof(HomeDevice) && typeof(HomeDevice).IsAssignableFrom(t)).Select(t => t.Name).ToArray();

                return homeDeviceTypes;
            }
        }

        [NotMapped]
        public string HomeDeviceType
        {
            get
            {
                return this.GetType().Name;
            }
        }

        [NotMapped]
        public bool InUse
        {
            get
            {
                return Connector != null;
            }
        }

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
