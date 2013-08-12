#region Using Statements
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
        public int? Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.LastValue = this.value.Value;
                this.value = value;
            }
        }

        [NotMapped]
        public int LastValue { get; private set; }

        private int? value;

        public Dimmable()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.Dimmer;
        }
    }
}
