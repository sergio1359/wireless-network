#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.Enums;
using SmartHome.Comunications.Messages;
using DataLayer.Entities.HomeDevices;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public class BusinessDoorLock
    {
        [OperationAttribute]        
        public static OperationMessage OpenDoor(this DoorLock doorLock)
        {
            return OperationMessage.LogicWrite(doorLock.Id, LogicWriteValues.Set, doorLock.OpenTime);
        }
    }
}
