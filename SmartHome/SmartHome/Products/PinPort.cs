using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Enums;

namespace SmartHome.Products
{
    class PinPort
    {
        //Direction
        public Char Port { set; get; }
        public Byte Pin { set; get; }

        public PinPort(Char Port, Byte Pin)
        {
            this.Port = Port;
            this.Pin = Pin;
        }

        public PinPort(String direction)
        {
            Port = direction[0];
            Pin = Byte.Parse(direction[1].ToString());
        }
    }

    class PinPortConfiguration
    {
        public const bool DEFAULT_OUTPUT = false; //Entrada
        public const bool DEFAULT_DIGITAL = true; //Digital


        public Boolean Output { get; set; }
        public Boolean Digital { get; set; }

        public enum Trigger : byte { None = 0x00, FallingEdge = 0x01, RisingEdge = 0x10, Both = 0x11 }

        //Digital-------------------------------------
        //output
        public Boolean DefaultValueD { get; set; }

        //input
        public Trigger changeTypeD { get; set; }


        //Analog-------------------------------------
        //output
        public Byte DefaultValueA { get; set; }

        //input
        public Byte Increment { get; set; }
        public Byte Threshold { get; set; }

        public PinPortConfiguration(Enums.HomeDeviceType HomeDeviceType)
        {
            switch (Enums.HomeDeviceType)
            {
                case Enums.HomeDeviceType.Buttom:
                    break;
                case Enums.HomeDeviceType.Dimmable:
                    break;
                case Enums.HomeDeviceType.DoorLock:
                    break;
                case Enums.HomeDeviceType.HumiditySensor:
                    break;
                case Enums.HomeDeviceType.LightSensor:
                    break;
                case Enums.HomeDeviceType.PowerSensor:
                    break;
                case Enums.HomeDeviceType.PresenceSensor:
                    break;
                case Enums.HomeDeviceType.RGBLight:
                    break;
                case Enums.HomeDeviceType.Switch:
                    break;
                case Enums.HomeDeviceType.TemperatureSensor:
                    break;
                case Enums.HomeDeviceType.WallPlug:
                    break;
                default:
                    break;
            }
        }
    }
}
