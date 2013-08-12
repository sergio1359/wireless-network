#region Using Statements
using DataLayer.Entities.Enums;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class DoorLock : HomeDevice
    {
        public const byte DEFAULT_OPEN_TIME = 1; //1 segundo

        public byte OpenTime { get; set; }

        public DoorLock()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.SwitchLOW;
        }
    }
}
