using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    class PresenceSensor: HomeDevice
    {
        public byte Sensibility { get; set; }
        public const int DEFAULT_SENSIBILITY = 10;

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
