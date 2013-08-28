#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications.Messages;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessPresenceSensor
    {
        public static OperationMessage RefreshState(this PresenceSensor presence)
        {
            ushort destinationAddress = (ushort)(presence.Connector == null ? 0 : presence.Connector.Node.Address);

            return OperationMessage.PresenceRead((ushort)presence.Id, destinationAddress);
        }
    }
}
