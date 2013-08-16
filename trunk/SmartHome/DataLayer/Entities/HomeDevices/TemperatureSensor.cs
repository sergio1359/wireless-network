#region Using Statements
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class TemperatureSensor: HomeDevice
    {
        [NotMapped]
        public int? CelciusTemperature { get; set; }

        public TemperatureSensor()
            : base()
        {
        }
    }
}
