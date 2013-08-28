#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications.Messages;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessHumiditySensor
    {
        public static OperationMessage RefreshState(this HumiditySensor humidity)
        {
            ushort destinationAddress = (ushort)(humidity.Connector == null ? 0 : humidity.Connector.Node.Address);

            return OperationMessage.HumidityRead((ushort)humidity.Id, destinationAddress);
        }
    }
}
