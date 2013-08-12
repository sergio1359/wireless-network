#region Using Statements
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class PowerSensor : HomeDevice
    {
        public const int DEFAULT_SENSIBILITY = 29;

        public byte Sensibility { set; get; }

        [NotMapped]
        public int? Consumption { set; get; }

        public PowerSensor()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.Dimmer;
        }
    }
}
