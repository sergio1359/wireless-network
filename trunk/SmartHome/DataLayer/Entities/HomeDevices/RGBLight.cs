#region Using Statements
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using DataLayer.Entities.HomeDevices.Status;
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

        [NotMapped]
        public ModeRGBLight? Mode
        {
            get
            {
                return this.ReadProperty<ModeRGBLight>("Mode");
            }
            set
            {
                this.StoreProperty("Mode", value);
            }
        }

        [NotMapped]
        public Color? Color
        {
            get
            {
                return this.ReadProperty<Color>("Color");
            }
            set
            {
                this.StoreProperty("Color", value);
            }
        }

        public RGBLight()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.RGB;

            this.DefaultDegradeTime = DEFAULT_DEGRADE_TIME;
        }
    }
}
