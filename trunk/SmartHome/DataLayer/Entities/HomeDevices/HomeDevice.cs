#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.Enums;
#endregion

namespace DataLayer.Entities.HomeDevices
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

        public ConnectorTypes ConnectorCapable { get; set; }

        public Location Location { get; set; }

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

        
    }
}
