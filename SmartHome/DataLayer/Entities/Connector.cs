#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.HomeDevices;
using DataLayer.Entities.Enums; 
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

        public Dictionary<HomeDevice, List<PinPort>> MappingHomeDevice;

        [NotMapped]
        public List<HomeDevice> HomeDevices
        {
            get
            {
                return MappingHomeDevice.Keys.ToList();
            }
        }

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
            MappingHomeDevice = new Dictionary<HomeDevice, List<PinPort>>();
        }
    }
}
