#region Using Statements
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DataLayer.Entities.HomeDevices.Status;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    [Table("PowerSensor")]
    public class PowerSensor : HomeDevice
    {
        public const int DEFAULT_SENSIBILITY = 29;

        [Range(0, 255)]
        public int Sensibility { set; get; }

        [NotMapped]
        [PropertyAttribute]
        public int? Consumption
        {
            get
            {
                return this.ReadProperty<int>("Consumption");
            }
            set
            {
                this.StoreProperty("Consumption", value);
            }
        }

        public PowerSensor()
            : base()
        {
            this.Sensibility = DEFAULT_SENSIBILITY;

            base.ConnectorCapable = ConnectorTypes.PowerSensor;
        }
    }
}
