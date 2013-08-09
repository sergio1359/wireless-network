using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    public class LuminositySensor : HomeDevice
    {
        public byte Sensibility { get; set; }

        public LuminositySensor()
            : base()
        {
        }


        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
