using SmartHome.Comunications;
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


        public OperationMessage On()
        {
            return PercentageDimmer(100);
        }

        public OperationMessage Off()
        {
            return PercentageDimmer(0);
        }

        public OperationMessage OnTime(byte seconds)
        {
            return PercentageDimmer(100, seconds);
        }

        public OperationMessage Switch()
        {
            if (Value != 0)
            {
                return Off();
            }
            else
            {
                return PercentageDimmer(lastValue);
            }
        }

        public OperationMessage PercentageDimmer(int percentage)
        {
            return PercentageDimmer(percentage, 0);
        }

        public OperationMessage PercentageDimmer(int percentage, byte seconds)
        {
            return OperationMessage.DimmerWrite(Id, (byte)(percentage * byte.MaxValue / 100.0), 0);
            //Value = percentage;
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
