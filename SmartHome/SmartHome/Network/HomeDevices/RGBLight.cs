using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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

        public void Invert(byte seconds)
        {
            Color invert = Color.FromArgb(this.Color.ToArgb() ^ 0xFFFFFF);
            ToColor(invert, seconds);
        }

        public void Random(byte seconds)
        {
            ColorWriteRandom(seconds);
        }

        public void RandomSecuence(Color[] colors, byte seconds)
        {
            ColorRandomSecuenceWrite(seconds, colors);
        }

        public void SortedSecuence(Color[] colors, byte seconds)
        {
            ColorSortedSecuenceWrite(seconds, colors);
        }

        public void ToColor(Color color, byte seconds)
        {
            ColorWrite(color, seconds);
        }

        public void TurnOff()
        {
            ToColor(Color.Black, 1);
        }

        public void White()
        {
            ToColor(Color.White, 1);
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
        
    }
}
