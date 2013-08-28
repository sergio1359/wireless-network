#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications.Messages;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessPowerSensor
    {
        public static OperationMessage RefreshState(this PowerSensor powerSensor)
        {
            ushort destinationAddress = (ushort)(powerSensor.Connector == null ? 0 : powerSensor.Connector.Node.Address);

            return OperationMessage.PowerRead((ushort)powerSensor.Id, destinationAddress);
        }
    }
}
