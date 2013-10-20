#region Using Statements
using DataLayer.Entities.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.HomeDevices.Status;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class SwitchButton : HomeDevice
    {
        [NotMapped]
        [PropertyAttribute]
        public bool? Open
        {
            get
            {
                return this.ReadProperty<bool>("Open");
            }
            set
            {
                this.StoreProperty("Open", value);
            }
        }

        public SwitchButton()
            : base()
        {
            base.Operations = new List<Operation>();
            base.ConnectorCapable = ConnectorTypes.LogicInput;
        }
    }
}
