#region Using Statements
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices.Status;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class Light : HomeDevice
    {
        [NotMapped]
        public bool? IsOn
        {
            get
            {
                return this.ReadProperty<bool>("IsOn");
            }
            set
            {
                this.StoreProperty("IsOn", value);
            }
        }

        public Light()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.SwitchLOW;
        }
    }
}
