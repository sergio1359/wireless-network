using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.Plugins;
using SmartHome.Network;

namespace SmartHome.Products
{
    public class Shield
    {

        public ShieldType ShieldType { set; get; }
        public Dictionary<string, List<PinPort>> PinPorts { set; get; }

        public Shield(ShieldType shieldtype)
        {
            ShieldType = shieldtype;
            
            switch (shieldtype)
            {
                case ShieldType.Example:
                    PinPorts.Add("Digital0",    new List<PinPort>() { new PinPort("A0") });
                    PinPorts.Add("Digital1",    new List<PinPort>() { new PinPort("A1") });
                    PinPorts.Add("Analog0",     new List<PinPort>() { new PinPort("F0") });
                    PinPorts.Add("Analog1",     new List<PinPort>() { new PinPort("F1") });
                    PinPorts.Add("Analog2",     new List<PinPort>() { new PinPort("F2") });
                    PinPorts.Add("PWM",         new List<PinPort>() { new PinPort("B4"), new PinPort("B7"), new PinPort("G5") });
                    PinPorts.Add("Dimmer0",     new List<PinPort>() { new PinPort("G0") });
                 default:
                    throw new Exception();
            }
        }
    }    
}
