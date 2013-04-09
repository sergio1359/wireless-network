using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    class WallPlug: HomeDevice
    {
        public enum WallPlugType
        {
            AirFreshener,
            LightPoint,
            Matamosquitos,
            Fan,
            Heater,
            Speaker,
            Other,
            None,
        }
        
        public bool Active { get; set; }
        public WallPlugType Type { get; set; }
        

        public void On(){}
        public void Off(){}
        public void ONTime() { }
        public void Switch() { }
        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
