#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications.Messages;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessTemperatureSensor
    {
        public static OperationMessage RefreshState(this TemperatureSensor temperatureSensor)
        {
            ushort destinationAddress = (ushort)(temperatureSensor.Connector == null ? 0 : temperatureSensor.Connector.Node.Address);

            return OperationMessage.TemperatureRead((ushort)temperatureSensor.Id, destinationAddress);
        }
    }
}
