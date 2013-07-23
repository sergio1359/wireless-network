using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generate_Simple_EEPROM
{
    class Program
    {
        static void Main(string[] args)
        {

            #region Example 01
            ////Example 01: pulsar y que el led se apague
            //NetworkManager.Nodes.Add(new Node(0x00, BaseType.ATMega128RFA1_V1, ShieldType.Debug));
            //NetworkManager.HomeDevices.Add(new Button("Botón Placa"));
            //NetworkManager.HomeDevices.Add(new Light("Led"));

            //NetworkManager.Nodes[0].Connectors[0].HomeDevice = NetworkManager.HomeDevices[0];
            //NetworkManager.Nodes[0].Connectors[1].HomeDevice = NetworkManager.HomeDevices[1];

            ////Puke: bidirectional
            //NetworkManager.Nodes[0].Connectors[0].HomeDevice.Connector = NetworkManager.Nodes[0].Connectors[0];
            //NetworkManager.Nodes[0].Connectors[1].HomeDevice.Connector = NetworkManager.Nodes[0].Connectors[1];

            ////añadir nueva accion
            //NetworkManager.Nodes[0].Connectors[0].HomeDevice.Actions.Add(new SmartHome.Network.Action()
            //{
            //    OPCode = SmartHome.Messages.OPCode.DigitalSwitch,
            //    Args = new byte[] {0x03, (1<<6), 0x00}, //Hardconding: D6, Time = 0
            //    ToHomeDevice = NetworkManager.Nodes[0].Connectors[1].HomeDevice,
            //});

            ////escribir todas las eeproms
            //NetworkManager.GetAllEEPROMS(); 
            #endregion

            #region Example 02
            ////Example 02: pulsar que otro led se apague
            //NetworkManager.Nodes.Add(new Node(0x00, BaseType.ATMega128RFA1_V1, ShieldType.Debug));
            //NetworkManager.HomeDevices.Add(new Button("Botón Placa"));
            //NetworkManager.HomeDevices.Add(new Light("Led"));

            //NetworkManager.Nodes[0].Connectors[0].HomeDevice = NetworkManager.HomeDevices[0];
            //NetworkManager.Nodes[0].Connectors[1].HomeDevice = NetworkManager.HomeDevices[1];

            ////Puke: bidirectional
            //NetworkManager.Nodes[0].Connectors[0].HomeDevice.Connector = NetworkManager.Nodes[0].Connectors[0];
            //NetworkManager.Nodes[0].Connectors[1].HomeDevice.Connector = NetworkManager.Nodes[0].Connectors[1];

            ////añadir nueva accion
            //NetworkManager.Nodes[0].Connectors[0].HomeDevice.Actions.Add(new SmartHome.Network.Action()
            //{
            //    OPCode = SmartHome.Messages.OPCode.DigitalSwitch,
            //    Args = new byte[] { 0x03, (1 << 6), 0x00 }, //Hardconding: D6, Time = 0
            //    ToHomeDevice = NetworkManager.Nodes[0].Connectors[1].HomeDevice,
            //    TimeRestrictions = new List<TimeRestriction>() { new TimeRestriction((byte)SmartHome.Network.TimeRestriction.WeekDays.Friday, 0, 0, 1, 0) }
            //});

            ////escribir todas las eeproms
            //NetworkManager.GetAllEEPROMS(); 
            #endregion

            #region Example 03
            ////Example 03: pulsar que otro led se apague
            //NetworkManager.Security.Channel = 0x0f;
            //NetworkManager.Security.PanId = 0x1234;

            //NetworkManager.Nodes.Add(new Node(0x00, BaseType.ATMega128RFA1_V1, ShieldType.Debug));
            //NetworkManager.Nodes.Add(new Node(0xFF, BaseType.ATMega128RFA1_V1, ShieldType.Debug));

            //NetworkManager.HomeDevices.Add(new Button("Botón Placa0"));
            //NetworkManager.HomeDevices.Add(new Light("Led0"));

            //NetworkManager.HomeDevices.Add(new Button("Botón Placa1"));
            //NetworkManager.HomeDevices.Add(new Light("Led1"));

            ////IDS HomeDevices
            //for (ushort i = 0; i < NetworkManager.HomeDevices.Count; i++)
            //{
            //    NetworkManager.HomeDevices[i].Id = i;
            //}

            ////Node0
            //NetworkManager.Nodes[0].Connectors[0].HomeDevice = NetworkManager.HomeDevices[0];
            //NetworkManager.Nodes[0].Connectors[1].HomeDevice = NetworkManager.HomeDevices[1];
            ////Puke: bidirectional
            //NetworkManager.Nodes[0].Connectors[0].HomeDevice.Connector = NetworkManager.Nodes[0].Connectors[0];
            //NetworkManager.Nodes[0].Connectors[1].HomeDevice.Connector = NetworkManager.Nodes[0].Connectors[1];
            //NetworkManager.Nodes[0].Address = 0x4003;


            ////Node1
            //NetworkManager.Nodes[1].Connectors[0].HomeDevice = NetworkManager.HomeDevices[2];
            //NetworkManager.Nodes[1].Connectors[1].HomeDevice = NetworkManager.HomeDevices[3];
            ////Puke: bidirectional
            //NetworkManager.Nodes[1].Connectors[0].HomeDevice.Connector = NetworkManager.Nodes[1].Connectors[0];
            //NetworkManager.Nodes[1].Connectors[1].HomeDevice.Connector = NetworkManager.Nodes[1].Connectors[1];
            //NetworkManager.Nodes[1].Address = 0x4004;

            ////Actions 0->1
            //NetworkManager.Nodes[0].Connectors[0].HomeDevice.Operations.Add(new Operation()
            //{
            //    OPCode = SmartHome.Comunications.OPCode.LogicSwitch,
            //    Args = new byte[] { 0x03, 0x00 }, //Harcondig: IDHomeDev, Time = 0
            //    DestionationHomeDevice = NetworkManager.Nodes[1].Connectors[1].HomeDevice,
            //});

            ////1->0
            //NetworkManager.Nodes[1].Connectors[0].HomeDevice.Operations.Add(new Operation()
            //{
            //    OPCode = SmartHome.Comunications.OPCode.LogicSwitch,
            //    Args = new byte[] { 0x01, 0x00 }, //Hardconding: IDHomeDev, Time = 0
            //    DestionationHomeDevice = NetworkManager.Nodes[0].Connectors[1].HomeDevice,
            //});

            //NetworkManager.GetAllEEPROMS(); 
            #endregion

            #region Example 04
            //Example 04: Configuracion para el "SensorShield" demostrativo
            NetworkManager.Security.Channel = 0x0f;
            NetworkManager.Security.PanId = 0x1234;

            NetworkManager.Nodes.Add(new Node(0x00, BaseType.ATMega128RFA1_V1, ShieldType.Debug));
            NetworkManager.Nodes.Add(new Node(0xFF, BaseType.ATMega128RFA1_V1, ShieldType.Debug));

            NetworkManager.HomeDevices.Add(new Button("Botón Placa0"));
            NetworkManager.HomeDevices.Add(new Light("Led0"));
            NetworkManager.HomeDevices.Add(new TemperatureSensor("TemperatureSensor0"));
            NetworkManager.HomeDevices.Add(new HumiditySensor("HumiditySensor0"));
            NetworkManager.HomeDevices.Add(new PresenceSensor("PresenceSensor0"));

            NetworkManager.HomeDevices.Add(new Button("Botón Placa1"));
            NetworkManager.HomeDevices.Add(new Light("Led1"));
            NetworkManager.HomeDevices.Add(new TemperatureSensor("TemperatureSensor1"));
            NetworkManager.HomeDevices.Add(new HumiditySensor("HumiditySensor1"));
            NetworkManager.HomeDevices.Add(new PresenceSensor("PresenceSensor1"));

            //IDS HomeDevices
            for (ushort i = 0; i < NetworkManager.HomeDevices.Count; i++)
            {
                NetworkManager.HomeDevices[i].Id = i;
            }

            //Node0
            NetworkManager.Nodes[0].Connectors[0].HomeDevice = NetworkManager.HomeDevices[0];
            NetworkManager.Nodes[0].Connectors[1].HomeDevice = NetworkManager.HomeDevices[1];
            NetworkManager.Nodes[0].Connectors[2].HomeDevice = NetworkManager.HomeDevices[2];
            NetworkManager.Nodes[0].Connectors[3].HomeDevice = NetworkManager.HomeDevices[3];
            NetworkManager.Nodes[0].Connectors[4].HomeDevice = NetworkManager.HomeDevices[4];

            NetworkManager.Nodes[0].Address = 0x4003;


            //Node1
            NetworkManager.Nodes[1].Connectors[0].HomeDevice = NetworkManager.HomeDevices[5];
            NetworkManager.Nodes[1].Connectors[1].HomeDevice = NetworkManager.HomeDevices[6];
            NetworkManager.Nodes[1].Connectors[2].HomeDevice = NetworkManager.HomeDevices[7];
            NetworkManager.Nodes[1].Connectors[3].HomeDevice = NetworkManager.HomeDevices[8];
            NetworkManager.Nodes[1].Connectors[4].HomeDevice = NetworkManager.HomeDevices[9];

            NetworkManager.Nodes[1].Address = 0x4004;

            //Puke: bidirectional
            foreach (Node n in NetworkManager.Nodes)
            {
                foreach (Connector c in n.Connectors)
                {
                    if (c.HomeDevice != null)
                        c.HomeDevice.Connector = c;
                }
            }

            //Operations 0->1
            NetworkManager.Nodes[0].Connectors[0].HomeDevice.Operations.Add(new Operation()
            {
                OPCode = SmartHome.Comunications.OPCode.LogicSwitch,
                Args = new byte[] { 0x06, 0x00, 0x00 }, //Harcondig: IDHomeDev, Time = 0
                DestionationHomeDevice = NetworkManager.Nodes[1].Connectors[1].HomeDevice,
            });

            //1->0
            NetworkManager.Nodes[1].Connectors[0].HomeDevice.Operations.Add(new Operation()
            {
                OPCode = SmartHome.Comunications.OPCode.LogicSwitch,
                Args = new byte[] { 0x01, 0x00, 0x00 }, //Hardconding: IDHomeDev, Time = 0
                DestionationHomeDevice = NetworkManager.Nodes[0].Connectors[1].HomeDevice,
            });

            NetworkManager.GetAllEEPROMS();
            #endregion


            Console.WriteLine("Archivos de configuración generados.");
            Console.ReadLine();
        }
    }
}
