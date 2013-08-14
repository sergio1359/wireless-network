#region Using Statements
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

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

        [NotMapped]
        public bool? Connected { get; set; }

        public WallPlug()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.SwitchHI;
        }
    }
}
