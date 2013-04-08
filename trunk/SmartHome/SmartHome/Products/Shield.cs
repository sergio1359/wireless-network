using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.Enums;

namespace SmartHome.Products
{
    class Shield
    {

        public Enums.ShieldType ShieldType { set; get; }
        public Dictionary<string, List<PinPort>> PinPorts { set; get; }

        public Shield(Enums.ShieldType shieldtype)
        {
            ShieldType = shieldtype;
            
            switch (shieldtype)
            {
                case ShieldType.Example:
                    AddConnector("Digital0", Enums.ConnectorType.IODigital, new String[] { "A0" });
                    AddConnector("Digital1", Enums.ConnectorType.IODigital, new String[] { "A1" });
                    AddConnector("Analog0", Enums.ConnectorType.Analog, new String[] { "F0" });
                    AddConnector("Analog1", Enums.ConnectorType.Analog, new String[] { "F1" });
                    AddConnector("Analog2", Enums.ConnectorType.Analog, new String[] { "F2" });
                    AddConnector("PWM", Enums.ConnectorType.IODigital, new String[] { "B4", "B7", "G5" });
                    AddConnector("Dimmer0", Enums.ConnectorType.Dimmer, new String[] { "G0" });
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
            return Connectors.FirstOrDefault<Connector>(x => x.Directions.Exists(y => y.Pin == pin && y.Port == port));
        }

        public Connector GetConector(Byte port, Byte pin)
        {
            return Connectors.FirstOrDefault<Connector>(x => x.Directions.Exists(y => y.Pin == pin && y.Port - 'A' == port));
        }

        public PinPort GetPinPort(Char port, Byte pin)
        {
            if (GetConector(port, pin) == null)
                return null;
            else
                return GetConector(port, pin).Directions.FirstOrDefault<PinPort>(x => x.Port == port && x.Pin == pin);
        }

        public PinPort GetPinPort(Byte port, Byte pin)
        {
            if (GetConector(port, pin) == null)
                return null;
            else
                return GetConector(port, pin).Directions.FirstOrDefault<PinPort>(x => x.Port - 'A' == port && x.Pin == pin);
        }
    }    
}
