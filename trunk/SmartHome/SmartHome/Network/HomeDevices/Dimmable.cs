using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    class Dimmable: HomeDevice
    {
        public enum DimmableType
        {
            DimmerLight,
            Fan,
            Other,
        }

        public int Value { get; set; } //it's a value between 0 to 100
        public int LastValue { get; set; }
        public DimmableType Type { get; set; }


        public void On() { }
        public void Off() { }
        public void ONTime() { }
        public void Switch() { }
        public void PercentageDimmer(int percentage) { }
        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
