#region Using Statements
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.Enums;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class Light : HomeDevice
    {
        [NotMapped]
        public bool? IsOn { get; set; }

        public Light()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.SwitchLOW;
        }
    }
}
