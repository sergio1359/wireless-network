#region Using Statements
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public enum ModeRGBLight : byte
    {
        None = 0x00,
        RandomSecuence,
        Solid,
        SortedSecuence,
    }

    [Table("RGBLight")]
    public class RGBLight : HomeDevice
    {
        public const int DEFAULT_DEGRADE_TIME = 1;

        [Range(0, 255)]
        public int DefaultDegradeTime { get; set; }

        public ModeRGBLight Mode { get; set; }

        [NotMapped]
        public Color Color { get; set; }

        public RGBLight()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.RGB;

            this.DefaultDegradeTime = DEFAULT_DEGRADE_TIME;
        }
    }
}
