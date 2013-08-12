#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public class WallPlug: Light
    {
        public enum WallPlugType
        {
            AirFreshener,
            LightPoint,
            Matamosquitos,
            Fan,
            Heater,
            Speaker,
            Other,
            None,
        }
        
        public WallPlugType Type { get; set; }

        public WallPlug() : base() 
        {
            base.ConnectorCapable = ConnectorTypes.SwitchHI;
        }

        public override void RefreshState()
        {
            base.RefreshState();
        }
    }
}
