#region Using Statements
using DataLayer.Entities.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System;
using DataLayer.Entities.HomeDevices.Status;
using System.Data.Objects;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public abstract class HomeDevice
    {
        [Key]
        [Range(0, 0xFFFF)]
        public int Id { get; set; }

        public string Name { get; set; }

        public ConnectorTypes ConnectorCapable { get; set; }

        public virtual Connector Connector { get; set; }

        public virtual ICollection<Location> Locations { get; set; }

        [InverseProperty("SourceHomeDevice")]
        public virtual ICollection<Operation> Operations { get; set; }

        /// <summary>
        /// When is not empty, indicates the location of this HomeDevice in the product.
        /// </summary>
        public int? ProductTag { get; set; }

        private static string[] homeDeviceTypes = null;

        [NotMapped]
        public DateTime? LastStatusUpdate 
        {
            get
            {
                return this.ReadProperty<DateTime>("LastStatusUpdate");
            }
            set
            {
                this.StoreProperty("LastStatusUpdate", value);
            }
        }

        [NotMapped]
        public static string[] HomeDeviceTypes
        {
            get
            {
                if (homeDeviceTypes == null)
                    homeDeviceTypes = typeof(HomeDevice).Assembly.GetTypes()
                                                                .Where(t => t != typeof(HomeDevice) && typeof(HomeDevice).IsAssignableFrom(t))
                                                                .Select(t => t.Name)
                                                                .ToArray();

                return homeDeviceTypes;
            }
        }

        [NotMapped]
        public string HomeDeviceType
        {
            get
            {
                return ObjectContext.GetObjectType(this.GetType()).Name;
                //return this.GetType().Name;
            }
        }

        [NotMapped]
        public bool InUse
        {
            get
            {
                return this.Connector != null;
            }
        }

        public HomeDevice()
        {
            this.Locations = new List<Location>();
            this.Operations = new List<Operation>();
        }


    }
}
