#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Comunications.Messages;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessLight
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
    }
}
