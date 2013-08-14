#region Using Statements
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    [Table("HumiditySensor")]
    public class LuminositySensor : HomeDevice
    {
        [Range(0, 255)]
        public int Sensibility { get; set; }

        [NotMapped]
        public int? Luminosity { get; set; }

        public LuminositySensor()
            : base()
        {

        }
    }
}
