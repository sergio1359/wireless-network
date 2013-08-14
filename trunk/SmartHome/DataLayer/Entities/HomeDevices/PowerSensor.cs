#region Using Statements
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public int? Consumption { set; get; }

        public PowerSensor()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.Dimmer;

            this.Sensibility = DEFAULT_SENSIBILITY;
        }
    }
}
