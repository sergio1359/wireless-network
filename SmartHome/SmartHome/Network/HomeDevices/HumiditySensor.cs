using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    public class HumiditySensor: HomeDevice
    {
        public int Humidity { get; set; }

        public HumiditySensor(string name)
            : base(name)
        {
            base.Operations = new List<Operation>();
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
