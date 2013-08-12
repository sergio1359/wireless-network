#region Using Statements
using System.Drawing;
using DataLayer.Entities.Enums;
using SmartHome.Comunications.Messages;
using DataLayer.Entities.HomeDevices; 
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessRGBLight
    {

        [OperationAttribute]        
        public static OperationMessage Invert(this RGBLight rgbLight, byte seconds)
        {
            Color invert = Color.FromArgb(rgbLight.Color.ToArgb() ^ 0xFFFFFF);
            return rgbLight.ToColor(invert, seconds);
        }

        [OperationAttribute]
        public static OperationMessage Random(this RGBLight rgbLight, byte seconds)
        {
            return OperationMessage.ColorWriteRandom(rgbLight.Id, seconds);
        }

        [OperationAttribute]
        public static OperationMessage RandomSecuence(this RGBLight rgbLight, Color[] colors, byte seconds)
        {
            return OperationMessage.ColorRandomSecuenceWrite(rgbLight.Id, seconds, colors);
        }

        [OperationAttribute]
        public static OperationMessage SortedSecuence(this RGBLight rgbLight, Color[] colors, byte seconds)
        {
            return OperationMessage.ColorSortedSecuenceWrite(rgbLight.Id, seconds, colors);
        }

        [OperationAttribute]
        public static OperationMessage ToColor(this RGBLight rgbLight, Color color, byte seconds)
        {
            return OperationMessage.ColorWrite(rgbLight.Id, color, seconds);
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
    }
}
