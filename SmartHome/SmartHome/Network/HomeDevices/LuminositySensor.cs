using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    class LuminositySensor: HomeDevice
    {
        public int Sensibility { get; set; }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
