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
            ushort destinationAddress = (ushort)(light.Connector == null ? 0 : light.Connector.Node.Address);

            return OperationMessage.LogicWrite((ushort)light.Id, LogicWriteValues.Set, 0, destinationAddress);
        }

        [OperationAttribute]
        public static OperationMessage Off(this Light light)
        {
            ushort destinationAddress = (ushort)(light.Connector == null ? 0 : light.Connector.Node.Address);

            return OperationMessage.LogicWrite((ushort)light.Id, LogicWriteValues.Clear, 0, destinationAddress);
        }

        [OperationAttribute]
        public static OperationMessage OnTime(this Light light, byte seconds)
        {
            ushort destinationAddress = (ushort)(light.Connector == null ? 0 : light.Connector.Node.Address);

            return OperationMessage.LogicWrite((ushort)light.Id, LogicWriteValues.Set, seconds, destinationAddress);
        }

        [OperationAttribute]
        public static OperationMessage Switch(this Light light)
        {
            ushort destinationAddress = (ushort)(light.Connector == null ? 0 : light.Connector.Node.Address);

            return OperationMessage.LogicSwitch((ushort)light.Id, 0, destinationAddress);
        }

        public static OperationMessage RefreshState(this Light light)
        {
            ushort destinationAddress = (ushort)(light.Connector == null ? 0 : light.Connector.Node.Address);

            return OperationMessage.LogicRead((ushort)light.Id, destinationAddress);
        }
    }
}
