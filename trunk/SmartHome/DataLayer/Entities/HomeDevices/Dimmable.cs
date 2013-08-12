#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.Enums;
#endregion

namespace DataLayer.Entities.HomeDevices
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
            base.ConnectorCapable = ConnectorTypes.Dimmer;
        }
    }
}
