using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    public class TemperatureSensor: HomeDevice
    {
        public int CelciusTemperature { get; set; }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
