#region Using Statements
using System.Drawing;
using SmartHome.Comunications.Messages; 
#endregion

namespace SmartHome.Network.HomeDevices
{
    public class RGBLight: HomeDevice
    {
        public enum ModeRGBLight : byte {
            None = 0x00,
            RandomSecuence,
            Solid,
            SortedSecuence,
        }
        
        public int DefaultDegradeTime {get; set;} //en segundos?
        public ModeRGBLight Mode { get; set; }
        public Color Color { get; set; }

        public RGBLight() : base() 
        {
            base.ConnectorCapable = ConnectorType.RGB;
        }

        [OperationAttribute]        public OperationMessage Invert(byte seconds)
        {
            Color invert = Color.FromArgb(this.Color.ToArgb() ^ 0xFFFFFF);
            return ToColor(invert, seconds);
        }

        [OperationAttribute]
        public OperationMessage Random(byte seconds)
        {
            return OperationMessage.ColorWriteRandom(Id, seconds);
        }

        [OperationAttribute]
        public OperationMessage RandomSecuence(Color[] colors, byte seconds)
        {
            return OperationMessage.ColorRandomSecuenceWrite(Id, seconds, colors);
        }

        [OperationAttribute]
        public OperationMessage SortedSecuence(Color[] colors, byte seconds)
        {
            return OperationMessage.ColorSortedSecuenceWrite(Id, seconds, colors);
        }

        [OperationAttribute]
        public OperationMessage ToColor(Color color, byte seconds)
        {
            return OperationMessage.ColorWrite(Id, color, seconds);
        }

        [OperationAttribute]
        public OperationMessage TurnOff()
        {
            return ToColor(Color.Black, 1);
        }

        [OperationAttribute]
        public OperationMessage White()
        {
            return ToColor(Color.White, 1);
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
        
    }
}
