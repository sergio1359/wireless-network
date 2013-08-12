#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class Dimmable : HomeDevice
    {
        public enum DimmableType
        {
            DimmerLight,
            Fan,
            Other,
        }

        public DimmableType Type { get; set; }

        [NotMapped]
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
        }

        private int lastValue;
        private int value;

        public Dimmable()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.Dimmer;
        }
    }
}
