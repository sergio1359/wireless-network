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
    public class BusinessLight
    {
        [OperationAttribute]
        public static OperationMessage On(this Light light)
        {
            return OperationMessage.LogicWrite(light.Id, LogicWriteValues.Set, 0);
        }

        [OperationAttribute]
        public static OperationMessage Off(this Light light)
        {
            return OperationMessage.LogicWrite(light.Id, LogicWriteValues.Clear, 0);
        }

        [OperationAttribute]
        public static OperationMessage OnTime(this Light light, byte seconds)
        {
            return OperationMessage.LogicWrite(light.Id, LogicWriteValues.Set, seconds);
        }

        [OperationAttribute]
        public static OperationMessage Switch(this Light light)
        {
            return OperationMessage.LogicSwitch(light.Id, 0);
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
        
    }
}
