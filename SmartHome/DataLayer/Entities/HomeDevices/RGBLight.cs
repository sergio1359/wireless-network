#region Using Statements
using System.Drawing;
using DataLayer.Entities.Enums; 
#endregion

namespace DataLayer.Entities.HomeDevices
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
            base.ConnectorCapable = ConnectorTypes.RGB;
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
        
    }
}
