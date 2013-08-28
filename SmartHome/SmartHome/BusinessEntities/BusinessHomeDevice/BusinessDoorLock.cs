#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications.Messages;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessDoorLock
    {
        [OperationAttribute]        
        public static OperationMessage OpenDoor(this DoorLock doorLock)
        {
            ushort destinationAddress = (ushort)(doorLock.Connector == null ? 0 : doorLock.Connector.Node.Address);

            return OperationMessage.LogicWrite((ushort)doorLock.Id, LogicWriteValues.Set, (byte)doorLock.OpenTime, destinationAddress);
        }
    }
}
