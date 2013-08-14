#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Comunications.Messages;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessDoorLock
    {
        [OperationAttribute]        
        public static OperationMessage OpenDoor(this DoorLock doorLock)
        {
            return OperationMessage.LogicWrite((ushort)doorLock.Id, LogicWriteValues.Set, (byte)doorLock.OpenTime);
        }
    }
}
