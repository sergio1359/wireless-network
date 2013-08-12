#region Using Statements
using System.Drawing;
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class RGBLight : HomeDevice
    {
        public const int DEFAULT_DEGRADE_TIME = 1;

        public enum ModeRGBLight : byte
        {
            None = 0x00,
            RandomSecuence,
            Solid,
            SortedSecuence,
        }

        public int DefaultDegradeTime { get; set; }

        public ModeRGBLight Mode { get; set; }

        [NotMapped]
        public Color Color { get; set; }

        public RGBLight()
            : base()
        {
            this.DefaultDegradeTime = DEFAULT_DEGRADE_TIME;
            base.ConnectorCapable = ConnectorTypes.RGB;
        }
    }
}
