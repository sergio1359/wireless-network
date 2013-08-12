#region Using Statements

#endregion

using System.ComponentModel.DataAnnotations.Schema;
namespace DataLayer.Entities.HomeDevices
{
    public class LuminositySensor : HomeDevice
    {
        public byte Sensibility { get; set; }

        [NotMapped]
        public int? Luminosity { get; set; }

        public LuminositySensor()
            : base()
        {

        }
    }
}
