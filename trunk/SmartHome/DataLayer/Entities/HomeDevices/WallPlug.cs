#region Using Statements
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.HomeDevices.Status;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public enum WallPlugTypes
    {
        None,
        Other,
        AirFreshener,
        LightPoint,
        Matamosquitos,
        Fan,
        Heater,
        Speaker,
    }

    [Table("WallPlug")]
    public class WallPlug : Light
    {
        public WallPlugTypes Type { get; set; }

        public WallPlug()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.SwitchHI;
        }
    }
}
