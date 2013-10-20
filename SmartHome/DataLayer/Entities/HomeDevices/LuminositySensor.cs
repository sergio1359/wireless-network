#region Using Statements
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.HomeDevices.Status;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    [Table("HumiditySensor")]
    public class LuminositySensor : HomeDevice
    {
        [Range(0, 255)]
        public int Sensibility { get; set; }

        [NotMapped]
        [PropertyAttribute]
        public int? Luminosity
        {
            get
            {
                return this.ReadProperty<int>("Luminosity");
            }
            set
            {
                this.StoreProperty("Luminosity", value);
            }
        }

        public LuminositySensor()
            : base()
        {
            base.ConnectorCapable = Enums.ConnectorTypes.LuminositySensor;
        }
    }
}
