#region Using Statements
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq; 
#endregion

namespace DataLayer.Entities
{
    public class Connector
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ConnectorTypes ConnectorType { get; set; }

        public virtual Node Node { get; set; }

        public virtual ICollection<HomeDevice> HomeDevices { get; set; }

        [NotMapped]
        public Dictionary<HomeDevice, List<PinPort>> MappingHomeDevice;

        [NotMapped]
        public List<PinPort> PinPorts
        {
            get
            {
                return MappingHomeDevice.Values.SelectMany(v => v).ToList(); //TODO: quitar los repetidos
            }
        }

        [NotMapped]
        public bool InUse
        {
            get
            {
                return HomeDevices.Count != 0;
            }
        }

        public Connector()
        {
            this.HomeDevices = new List<HomeDevice>();
            this.MappingHomeDevice = new Dictionary<HomeDevice, List<PinPort>>();
        }
    }
}
