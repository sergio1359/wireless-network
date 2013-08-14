#region Using Statements
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.Enums;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    [Table("DoorLock")]
    public class DoorLock : HomeDevice
    {
        public const byte DEFAULT_OPEN_TIME = 1; //1 segundo

        [Range(0, 255)]
        public int OpenTime { get; set; }

        public DoorLock()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.SwitchLOW;
            this.OpenTime = DEFAULT_OPEN_TIME;
        }
    }
}
