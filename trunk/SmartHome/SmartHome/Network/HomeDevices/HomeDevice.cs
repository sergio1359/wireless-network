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
        private static Dictionary<Type, string[]> homeDeviceOperations = null;

        [NotMapped]
        private static string[] homeDeviceTypes = null;

        [Key]
        public ushort Id { get; set; }

        public string Name { get; set; }

        public Connector Connector { get; set; }

        public ConnectorType ConnectorCapable { get; set; }

        public Position Position { get; set; }

        public List<Operation> Operations { get; set; }

        [NotMapped]
        public static string[] HomeDeviceTypes
        {
            get
            {
                if (homeDeviceTypes == null)
                    homeDeviceTypes = typeof(HomeDevice).Assembly.GetTypes().Where(t => t != typeof(HomeDevice) && typeof(HomeDevice).IsAssignableFrom(t)).Select(t => t.Name).ToArray();

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

        public static string[] GetHomeDeviceOperations(Type HomeDeviceType)
        {
            if (HomeDeviceType == null || !typeof(HomeDevice).IsAssignableFrom(HomeDeviceType))
                return null;

            if (homeDeviceOperations == null)
                homeDeviceOperations = new Dictionary<Type, string[]>();

            if (!homeDeviceOperations.ContainsKey(HomeDeviceType))
            {
                homeDeviceOperations.Add(HomeDeviceType,
                                        HomeDeviceType.GetMethods()
                                        .Where(m => m.GetCustomAttributes(true)
                                            .OfType<OperationAttribute>()
                                            .Where(a => !a.Internal).Count() > 0)
                                        .Select(m => m.Name)
                                        .ToArray());
            }

            return homeDeviceOperations[HomeDeviceType];
        }
    }
}
