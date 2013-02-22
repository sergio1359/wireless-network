using System;
using System.Collections.Generic;
using System.Linq;

namespace DomoticNetwork.NetworkModel
{
    class Shield
    {
        public Base ShieldBase { set; get; }
        public ShieldType Type { set; get; }
        public enum ShieldType : byte { Central = 0x00, Roseta = 0x01, Regleta = 0x02, Lampara = 0x03 };
        public List<Connector> Connectors { set; get; }
        public List<TimeEvent> TimeEvents { set; get; }

        public Shield(ShieldType shieldtype, Base.UControllerType basetype)
        {
            Type = shieldtype;
            ShieldBase = new Base(basetype);
            Connectors = new List<Connector>();
            TimeEvents = new List<TimeEvent>();

            switch (shieldtype)
            {
                case ShieldType.Central:
                    break;
                case ShieldType.Roseta:
                    //Definimos un tipo Roseta generico: 2 digitales, 2 analogicos, 3 PWM, 1 dimmer
                    AddConnector("Digital0", Enums.ConnectorType.IODigital, new String[] { "A0" });
                    AddConnector("Digital1", Enums.ConnectorType.IODigital, new String[] { "A1" });
                    AddConnector("Analog0", Enums.ConnectorType.Analog, new String[] { "F0" });
                    AddConnector("Analog1", Enums.ConnectorType.Analog, new String[] { "F1" });
                    AddConnector("Analog2", Enums.ConnectorType.Analog, new String[] { "F2" });
                    AddConnector("PWM", Enums.ConnectorType.IODigital, new String[] { "B4", "B7", "G5" });
                    AddConnector("Dimmer0", Enums.ConnectorType.Dimmer, new String[] { "A0" });
                    break;
                case ShieldType.Regleta:
                    break;
                case ShieldType.Lampara:
                    break;
                default:
                    throw new Exception();
            }
        }



        public void AddConnector(String name, Enums.ConnectorType type, String[] directions)
        {
            foreach (String dir in directions)
            {
                if (!ShieldBase.IsConnectable(dir, type))
                    throw new Exception("El tipo de conector no es compatible en esa direccion del micro");
            }

            Connectors.Add(new Connector(name, type, directions));
        }

        public Connector GetConector(Char port, Byte pin)
        {
            return Connectors.FirstOrDefault<Connector>(x => x.Directions.Exists(y => y.Pin == pin && y.Port - 'A' == port));
        }

        public Connector GetConector(Byte port, Byte pin)
        {
            return Connectors.FirstOrDefault<Connector>(x => x.Directions.Exists(y => y.Pin == pin && y.Port == port));
        }

        public PinPort GetPinPort(Char port, Byte pin)
        {
            if (GetConector(port, pin) == null)
                return null;
            else
                return GetConector(port, pin).Directions.FirstOrDefault<PinPort>(x => x.Port - 'A' == port && x.Pin == pin);
        }

        public PinPort GetPinPort(Byte port, Byte pin)
        {
            if (GetConector(port, pin) == null)
                return null;
            else
                return GetConector(port, pin).Directions.FirstOrDefault<PinPort>(x => x.Port == port && x.Pin == pin);
        }

        public void AddTimeEvent(TimeEvent te)
        {
            TimeEvents.Add(te);
        }
    }

    class Base
    {
        //Identificador de micro
        public UInt32 DeviceSignature { private set; get; }
        public enum UControllerType { ATMega128RFA1 };
        public UControllerType UController { set; get; }

        //numero de puertos que tiene el micro, y cada puerto tiene NumPins pines
        public UInt16 NumPorts { set; get; }
        public Byte NumPins { set; get; }

        //Esta informacion debe de estar organizada por orden Alfabetico
        public String[] AnalogPorts { set; get; }
        public String[] PWMPorts { set; get; }
        public String[] UnavailablePorts { set; get; }

        //Endianidad
        public Boolean LittleEndian { set; get; }


        public Base(UControllerType controller)
        {
            UController = controller;
            switch (controller)
            {
                case UControllerType.ATMega128RFA1:
                    DeviceSignature = 128;
                    NumPorts = 7;
                    NumPins = 8;

                    AnalogPorts = new String[8] { "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7" };
                    PWMPorts = new String[8] { "B4", "B5", "B6", "B7", "E3", "E4", "E5", "G5" };  //VERSION MINOLO:{ "B4", "B7", "G5" } el B7 y el G5 estan compartidos con el mismo timer
                    UnavailablePorts = new String[2] { "G3", "G4" };  //TODO: de momento estos pero hay que chequear

                    LittleEndian = true;
                    break;
                default:
                    throw new Exception();
            }
        }

        public bool IsConnectable(String portPin, Enums.ConnectorType type)
        {
            if (!IsAvailabe(portPin))
                return false;
            if (type == Enums.ConnectorType.PWMTTL && !IsPWM(portPin))
                return false;
            if (type == Enums.ConnectorType.Analog && !IsAnalog(portPin))
                return false;
            return true;
        }

        /// <summary>
        /// Return true if IsAnalog
        /// </summary>
        /// <param name="portPin">Name of port and number of pin, Example: F3, A1</param>
        /// <returns></returns>
        public bool IsAnalog(String portPin)
        {
            return AnalogPorts.Any(x => x == portPin);
        }

        /// <summary>
        /// Return true if IsAnalog
        /// </summary>
        /// <param name="portPin">Name of port and number of pin, Example: F3, A1</param>
        /// <returns></returns>
        public bool IsPWM(String portPin)
        {
            return PWMPorts.Any(x => x == portPin);
        }

        /// <summary>
        /// Return true if available this pin
        /// </summary>
        /// <param name="portPin">Name of port and number of pin, Example: F3, A1</param>
        /// <returns></returns>
        public bool IsAvailabe(String portPin)
        {
            if (Int16.Parse(portPin[1].ToString()) > 7)
                return false;
            if (portPin[0] - 'A' + 1 > NumPorts || portPin[0] - 'A' < 0)
                return false;
            return !UnavailablePorts.Any(x => x == portPin);
        }
    }

    class Connector
    {
        public Enums.ConnectorType Type { set; get; }

        public String Name { set; get; }

        public List<PinPort> Directions { set; get; }

        //Events
        public List<BasicEvent> ConnectorEvent { set; get; }

        public Connector(String name, Enums.ConnectorType type, String[] directions)
        {
            Name = name;
            Type = type;
            Directions = new List<PinPort>();
            for (int i = 0; i < directions.Length; i++)
            {
                Directions.Add(new PinPort(i.ToString(), directions[i], type));
            }
            ConnectorEvent = new List<BasicEvent>();
        }
    }

    class PinPort
    {
        //Direction
        public String Name { set; get; }
        public Char Port { set; get; }
        public Byte Pin { set; get; }

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

        //pin events
        public List<BasicEvent> PinEvents { get; set; }

        public PinPort(String id, String direction, Enums.ConnectorType type)
        {
            Name = id;
            Port = direction[0];
            Pin = Byte.Parse(direction[1].ToString());

            switch (type)
            {
                case Enums.ConnectorType.SwitchHI:
                case Enums.ConnectorType.SwitchLOW:
                case Enums.ConnectorType.IODigital:
                case Enums.ConnectorType.Dimmer:
                    Output = true;
                    Digital = true;
                    DefaultValueD = true;
                    break;
                case Enums.ConnectorType.PWMTTL:
                    Output = true;
                    Digital = false;
                    DefaultValueA = 255;
                    break;
                case Enums.ConnectorType.Analog:
                    Output = false;
                    Digital = false;
                    Increment = 24;
                    Threshold = 128;
                    break;
                default:
                    throw new Exception("no implementado este tipo de conector");
            }
        }
    }
}
