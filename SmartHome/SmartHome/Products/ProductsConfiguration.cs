using DataLayer.Entities;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System;
using System.Collections.Generic;

namespace SmartHome.Products
{
    public class ProductConfiguration
    {
        public static Dictionary<string, Tuple<ConnectorTypes, List<PinPort>>> GetShieldDictionary(ShieldTypes shieldtype)
        {
            Dictionary<string, Tuple<ConnectorTypes, List<PinPort>>> pinPorts = new Dictionary<string, Tuple<ConnectorTypes, List<PinPort>>>();
            switch (shieldtype)
            {
                case ShieldTypes.Debug:
                    pinPorts.Add("Button",      new Tuple<ConnectorTypes, List<PinPort>>(ConnectorTypes.LogicInput, new List<PinPort>() { new PinPort("D7") }));
                    pinPorts.Add("Light",       new Tuple<ConnectorTypes, List<PinPort>>(ConnectorTypes.SwitchLOW, new List<PinPort>() { new PinPort("D6") }));
                    pinPorts.Add("Temperature", new Tuple<ConnectorTypes, List<PinPort>>(ConnectorTypes.LogicInput, new List<PinPort>() { new PinPort("E2") }));
                    pinPorts.Add("Humidity",    new Tuple<ConnectorTypes, List<PinPort>>(ConnectorTypes.LogicInput, new List<PinPort>() { new PinPort("E2") }));
                    pinPorts.Add("Presence",    new Tuple<ConnectorTypes, List<PinPort>>(ConnectorTypes.LogicInput, new List<PinPort>() { new PinPort("B6") }));
                    break;
                default:
                    break;
            }

            return pinPorts;
        }

        public static Base GetBaseConfiguration(BaseTypes controller)
        {
            Base result = new Base();
            switch (controller)
            {
                case BaseTypes.ATMega128RFA1_V1:
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

        public static PinPortConfiguration GetPinPortConfiguration(HomeDevice homeDevice)
        {
            PinPortConfiguration configuration = DefaultPinPortConfiguration();

            if (homeDevice is Button)
            {
                configuration.Output = false;
                configuration.Digital = true;
                configuration.ChangeTypeD = PinPortConfiguration.Trigger.FallingEdge;
            }
            else if (homeDevice is Switch)
            {
                configuration.Output = false;
                configuration.Digital = true;
                configuration.ChangeTypeD = PinPortConfiguration.Trigger.Both;
            }
            else if (homeDevice is Light || homeDevice is WallPlug)
            {
                configuration.Output = true;
                configuration.Digital = true;
                configuration.DefaultValueD = false;
            }
            else if (homeDevice is Dimmable)
            {
                configuration.Output = true;
                configuration.Digital = false;
                configuration.DefaultValueA = 0x00;
            }
            else if (homeDevice is PresenceSensor)
            {
                configuration.Output = false;
                configuration.Digital = true;
                configuration.ChangeTypeD = PinPortConfiguration.Trigger.RisingEdge;
            }
            else if(homeDevice is DoorLock)
            {
                configuration.Output = true;
                configuration.Digital = true;
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
