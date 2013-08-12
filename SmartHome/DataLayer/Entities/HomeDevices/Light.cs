#region Using Statements
using DataLayer.Entities.Enums;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class Light : HomeDevice
    {
        public bool IsOn { get; set; }

        public Light()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.SwitchLOW;
        }
    }
}
