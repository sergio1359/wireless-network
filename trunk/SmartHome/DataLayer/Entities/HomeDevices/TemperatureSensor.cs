#region Using Statements
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.HomeDevices.Status;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class TemperatureSensor: HomeDevice
    {
        [NotMapped]
        public int? CelciusTemperature
        {
            get
            {
                return this.ReadProperty<int>("CelciusTemperature");
            }
            set
            {
                this.StoreProperty("CelciusTemperature", value);
            }
        }

        public TemperatureSensor()
            : base()
        {
            base.ConnectorCapable = Enums.ConnectorTypes.TemperatureSensor;
        }
    }
}
