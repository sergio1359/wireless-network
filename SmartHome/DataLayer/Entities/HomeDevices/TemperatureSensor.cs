#region Using Statements
using System.Collections.Generic;

#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class TemperatureSensor: HomeDevice
    {
        public int CelciusTemperature { get; set; }

        public TemperatureSensor()
            : base()
        {
            base.Operations = new List<Operation>();
        }
    }
}
