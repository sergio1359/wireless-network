using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    class Dimmable : HomeDevice
    {
        public enum DimmableType
        {
            DimmerLight,
            Fan,
            Other,
        }

        private int value;
        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                lastValue = this.value;
                this.value = value;
            }
        } //it's a value between 0 to 100
        private int lastValue;

        public DimmableType Type { get; set; }


        public void On()
        {
            PercentageDimmer(100);
        }

        public void Off()
        {
            PercentageDimmer(0);
        }

        public void OnTime(byte seconds)
        {
            PercentageDimmer(100, seconds);
        }

        public void Switch()
        {
            if (Value != 0)
            {
                Off();
            }
            else
            {
                PercentageDimmer(lastValue);
            }
        }

        public void PercentageDimmer(int percentage)
        {
            PercentageDimmer(percentage, 0);
        }

        public void PercentageDimmer(int percentage, byte seconds)
        {
            DimmerWrite((byte)(percentage * byte.MaxValue / 100.0), 0);
            //Value = percentage;
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
