using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    class PowerSensor: HomeDevice
    {
        public int Sensibility { set; get; }
        public int MaximumConsumption { set; get; }
        public int CurrentConsumption { set; get; }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
