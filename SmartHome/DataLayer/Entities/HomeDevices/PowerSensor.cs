#region Using Statements
using DataLayer.Entities.Enums;

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class PowerSensor : HomeDevice
    {
        public const int DEFAULT_SENSIBILITY = 29;

        public byte Sensibility { set; get; }
        public int Consumption { set; get; }

        public PowerSensor()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.Dimmer;
        }
    }
}
