#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications.Messages;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessLuminositySensor
    {
        public static OperationMessage RefreshState(this LuminositySensor luminosity)
        {
            ushort destinationAddress = (ushort)(luminosity.Connector == null ? 0 : luminosity.Connector.Node.Address);

            return OperationMessage.LuminosityRead((ushort)luminosity.Id, destinationAddress);
        }
    }
}
