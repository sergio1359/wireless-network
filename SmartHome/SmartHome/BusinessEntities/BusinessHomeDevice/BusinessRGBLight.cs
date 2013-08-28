#region Using Statements
using DataLayer.Entities.HomeDevices;
using SmartHome.Comunications.Messages;
using System.Drawing; 
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessRGBLight
    {
        [OperationAttribute]
        public static OperationMessage Random(this RGBLight rgbLight, byte seconds)
        {
            ushort destinationAddress = (ushort)(rgbLight.Connector == null ? 0 : rgbLight.Connector.Node.Address);

            return OperationMessage.ColorWriteRandom((ushort)rgbLight.Id, seconds, destinationAddress);
        }

        [OperationAttribute]
        public static OperationMessage RandomSecuence(this RGBLight rgbLight, Color[] colors, byte seconds)
        {
            ushort destinationAddress = (ushort)(rgbLight.Connector == null ? 0 : rgbLight.Connector.Node.Address);

            return OperationMessage.ColorRandomSecuenceWrite((ushort)rgbLight.Id, seconds, colors, destinationAddress);
        }

        [OperationAttribute]
        public static OperationMessage SortedSecuence(this RGBLight rgbLight, Color[] colors, byte seconds)
        {
            ushort destinationAddress = (ushort)(rgbLight.Connector == null ? 0 : rgbLight.Connector.Node.Address);

            return OperationMessage.ColorSortedSecuenceWrite((ushort)rgbLight.Id, seconds, colors, destinationAddress);
        }

        [OperationAttribute]
        public static OperationMessage ToColor(this RGBLight rgbLight, Color color, byte seconds)
        {
            ushort destinationAddress = (ushort)(rgbLight.Connector == null ? 0 : rgbLight.Connector.Node.Address);

            return OperationMessage.ColorWrite((ushort)rgbLight.Id, color, seconds, destinationAddress);
        }

        [OperationAttribute]
        public static OperationMessage TurnOff(this RGBLight rgbLight)
        {
            return rgbLight.ToColor(Color.Black, 1);
        }

        [OperationAttribute]
        public static OperationMessage White(this RGBLight rgbLight)
        {
            return rgbLight.ToColor(Color.White, 1);
        }

        public static OperationMessage RefreshState(this RGBLight rgbLight)
        {
            ushort destinationAddress = (ushort)(rgbLight.Connector == null ? 0 : rgbLight.Connector.Node.Address);

            return OperationMessage.ColorRead((ushort)rgbLight.Id, destinationAddress);
        }
    }
}
