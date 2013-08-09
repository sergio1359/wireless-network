#region Using Statements
using SmartHome.Comunications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Comunications.Messages; 
#endregion

namespace SmartHome.Network.HomeDevices
{
    public class DoorLock : HomeDevice
    {
        public const byte DEFAULT_OPEN_TIME = 1; //1 segundo

        public byte OpenTime { get; set; }

        public DoorLock() : base() 
        {
            base.ConnectorCapable = ConnectorType.SwitchLOW;
        }

        [OperationAttribute]        public OperationMessage OpenDoor()
        {
            return OperationMessage.LogicWrite(Id, LogicWriteValues.Set, OpenTime);
        }
    }
}
