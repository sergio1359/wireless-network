#region Using Statements
using DataLayer.Entities.Enums;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class Button : HomeDevice
    {
        public Button()
            : base()
        {
            base.ConnectorCapable = ConnectorTypes.LogicInput;
        }

    }
}
