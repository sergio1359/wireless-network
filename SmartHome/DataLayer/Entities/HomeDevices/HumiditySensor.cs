#region Using Statements
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.HomeDevices.Status;

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class HumiditySensor : HomeDevice
    {
        [NotMapped]
        public int? Humidity
        {
            get
            {
                return this.ReadProperty<int>("Humidity");
            }
            set
            {
                this.StoreProperty("Humidity", value);
            }
        }

        public HumiditySensor()
            : base()
        {
            this.ConnectorCapable = Enums.ConnectorTypes.HumiditySensor;
        }
    }
}
