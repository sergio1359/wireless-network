#region Using Statements
using DataLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.HomeDevices.Status;
using System;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public enum DimmableTypes
    {
        DimmerLight,
        Fan,
        Other,
    }

    [Table("Dimmable")]
    public class Dimmable : HomeDevice
    {
        public DimmableTypes Type { get; set; }

        [NotMapped]
        [PropertyAttribute]
        public float? Value
        {
            get
            {
                return this.ReadProperty<float>("Value");
            }
            set
            {
                if (value.HasValue)
                    value = Math.Min(1f, Math.Max(0f, value.Value));

                if (this.Value != value)
                {
                    this.StoreProperty("LastValue", this.Value);
                    this.StoreProperty("Value", value);
                }
            }
        }

        [NotMapped]
        [PropertyAttribute]
        public float? LastValue
        {
            get
            {
                return this.ReadProperty<float>("LastValue");
            }
        }

        public Dimmable()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.Dimmer;
        }
    }
}
