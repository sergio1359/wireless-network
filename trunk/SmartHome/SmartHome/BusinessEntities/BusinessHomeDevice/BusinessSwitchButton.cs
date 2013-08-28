#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Comunications.Messages;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessSwitchButton
    {
        public static OperationMessage RefreshState(this SwitchButton switchButton)
        {
            ushort destinationAddress = (ushort)(switchButton.Connector == null ? 0 : switchButton.Connector.Node.Address);

            return OperationMessage.DimmerRead((ushort)switchButton.Id, destinationAddress);
        }
    }
}
