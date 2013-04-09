using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SmartHome.Network.HomeDevices
{
    class RGBLight: HomeDevice
    {
        public enum ModeRGBLight  {
            None,
            RandomSecuence,
            Solid,
            SortedSecuence,
        }
        
        public int DefaultDegradeTime {get; set;} //en segundos?
        public ModeRGBLight Mode { get; set; }
        public Color Color { get; set; }

        public override void RefreshState()
        {
            base.RefreshState();
        }
        
    }
}
