using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.Plugins;
using SmartHome.Network;

namespace SmartHome.Products
{
    public class ProductConfiguration
    {
        public static Dictionary<string, List<PinPort>> GetShieldDictionary(ShieldType shieldtype)
        {
            Dictionary<string, List<PinPort>> pinPorts = new Dictionary<string, List<PinPort>>();
            switch (shieldtype)
            {
                case ShieldType.Example:
                    pinPorts.Add("Digital0", new List<PinPort>() { new PinPort("A0") });
                    pinPorts.Add("Digital1", new List<PinPort>() { new PinPort("A1") });
                    pinPorts.Add("Analog0", new List<PinPort>() { new PinPort("F0") });
                    pinPorts.Add("Analog1", new List<PinPort>() { new PinPort("F1") });
                    pinPorts.Add("Analog2", new List<PinPort>() { new PinPort("F2") });
                    pinPorts.Add("PWM", new List<PinPort>() { new PinPort("B4"), new PinPort("B7"), new PinPort("G5") });
                    pinPorts.Add("Dimmer0", new List<PinPort>() { new PinPort("G0") });
                    break;

                case ShieldType.PinPortMap:
                    pinPorts.Add("A0", new List<PinPort>() { new PinPort("A0") });
                    pinPorts.Add("A1", new List<PinPort>() { new PinPort("A1") });
                    pinPorts.Add("A2", new List<PinPort>() { new PinPort("A2") });
                    pinPorts.Add("A3", new List<PinPort>() { new PinPort("A3") });
                    pinPorts.Add("A4", new List<PinPort>() { new PinPort("A4") });
                    pinPorts.Add("A5", new List<PinPort>() { new PinPort("A5") });
                    pinPorts.Add("A6", new List<PinPort>() { new PinPort("A6") });
                    pinPorts.Add("A7", new List<PinPort>() { new PinPort("A7") });
                    break;

                case ShieldType.Debug:
                    pinPorts.Add("Button", new List<PinPort>() { new PinPort("D7") });
                    pinPorts.Add("Light", new List<PinPort>() { new PinPort("D6") });
                    break;
                default:
                    break;
            }

            return pinPorts;
        }

        public static Base GetBaseConfiguration(BaseType controller)
        {
            Base result = new Base();
            switch (controller)
            {
                case BaseType.ATMega128RFA1_V1:
                    result.UController = controller;
                    result.DeviceSignature = 128;
                    result.NumPorts = 7;
                    result.NumPins = 8;

                    result.AnalogPorts = new string[8] { "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7" };
                    result.PWMPorts = new string[8] { "B4", "B5", "B6", "B7", "E3", "E4", "E5", "G5" };  //VERSION MINOLO:{ "B4", "B7", "G5" } el B7 y el G5 estan compartidos con el mismo timer
                    result.UnavailablePorts = new string[2] { "G3", "G4" };  //TODO: de momento estos pero hay que chequear

                    result.LittleEndian = true;
                    break;
                default:
                    throw new Exception();
            }

            return result;
        }

        public static PinPortConfiguration GetPinPortConfiguration(HomeDeviceType homeDeviceType)
        {
            PinPortConfiguration configuration = DefaultPinPortConfiguration();

            switch (homeDeviceType)
            {
                case HomeDeviceType.Buttom:
                    configuration.Output = false;
                    configuration.Digital = true;
                    configuration.ChangeTypeD = PinPortConfiguration.Trigger.FallingEdge;
                    break;
                case HomeDeviceType.Switch:
                    configuration.Output = false;
                    configuration.Digital = true;
                    configuration.ChangeTypeD = PinPortConfiguration.Trigger.Both;
                    break;
                case HomeDeviceType.WallPlug:
                case HomeDeviceType.Light:
                    configuration.Output = true;
                    configuration.Digital = true;
                    configuration.DefaultValueD = false;
                    break;
                case HomeDeviceType.Dimmable:
                    configuration.Output = true;
                    configuration.Digital = false;
                    configuration.DefaultValueA = 0x00;
                    break;
                case HomeDeviceType.PresenceSensor:
                    configuration.Output = false;
                    configuration.Digital = true;
                    configuration.ChangeTypeD = PinPortConfiguration.Trigger.RisingEdge;
                    break;
                case HomeDeviceType.PowerSensor:
                    break;
                case HomeDeviceType.TemperatureSensor:
                    break;
                case HomeDeviceType.HumiditySensor:
                    break;
                case HomeDeviceType.LightSensor:
                    break;
                case HomeDeviceType.DoorLock:
                    configuration.Output = true;
                    configuration.Digital = true;
                    break;
                case HomeDeviceType.RGBLight:
                    break;
                default:
                    break;
            }

            return configuration;
        }

        public static PinPortConfiguration DefaultPinPortConfiguration()
        {
            PinPortConfiguration configuration = new PinPortConfiguration();

            configuration.Digital = PinPortConfiguration.DEFAULT_DIGITAL;
            configuration.Output = PinPortConfiguration.DEFAULT_OUTPUT;

            configuration.ChangeTypeD = PinPortConfiguration.Trigger.None;
            configuration.DefaultValueD = false;

            configuration.DefaultValueA = 0x00;
            configuration.Increment = 0x00;
            configuration.Threshold = 0x00;

            return configuration;
        }
    }
}
