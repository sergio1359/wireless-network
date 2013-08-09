#region Using Statements
using SmartHome.Comunications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Comunications.Messages;
#endregion

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

        public Dimmable() : base() 
        {
            base.ConnectorCapable = ConnectorType.Dimmer;
        }

        [OperationAttribute]        
        public OperationMessage On()
        {
            return PercentageDimmer(100);
        }

        [OperationAttribute]
        public OperationMessage Off()
        {
            return PercentageDimmer(0);
        }

        [OperationAttribute]
        public OperationMessage OnTime(byte seconds)
        {
            return PercentageDimmer(100, seconds);
        }

        [OperationAttribute]
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

        [OperationAttribute]
        public OperationMessage PercentageDimmer(int percentage)
        {
            return PercentageDimmer(percentage, 0);
        }

        [OperationAttribute]
        public OperationMessage PercentageDimmer(int percentage, byte seconds)
        {
            return OperationMessage.DimmerWrite(Id, (byte)(percentage * byte.MaxValue / 100.0), 0);
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
