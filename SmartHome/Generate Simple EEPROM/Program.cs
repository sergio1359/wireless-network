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



            //Example 03: pulsar que otro led se apague
            NetworkManager.Security.Channel = 0x0f;
            NetworkManager.Security.PanId = 0x1234;

            NetworkManager.Nodes.Add(new Node(0x00, BaseType.ATMega128RFA1_V1, ShieldType.Debug));
            NetworkManager.Nodes.Add(new Node(0xFF, BaseType.ATMega128RFA1_V1, ShieldType.Debug));

            NetworkManager.HomeDevices.Add(new Button("Botón Placa0"));
            NetworkManager.HomeDevices.Add(new Light("Led0"));

            NetworkManager.HomeDevices.Add(new Button("Botón Placa1"));
            NetworkManager.HomeDevices.Add(new Light("Led1"));

            //IDS HomeDevices
            for (ushort i = 0; i < NetworkManager.HomeDevices.Count; i++)
            {
                NetworkManager.HomeDevices[i].ID = i;
            }

            //Node0
            NetworkManager.Nodes[0].Connectors[0].HomeDevice = NetworkManager.HomeDevices[0];
            NetworkManager.Nodes[0].Connectors[1].HomeDevice = NetworkManager.HomeDevices[1];
            //Puke: bidirectional
            NetworkManager.Nodes[0].Connectors[0].HomeDevice.Connector = NetworkManager.Nodes[0].Connectors[0];
            NetworkManager.Nodes[0].Connectors[1].HomeDevice.Connector = NetworkManager.Nodes[0].Connectors[1];
            NetworkManager.Nodes[0].Address = 0x4003;


            //Node1
            NetworkManager.Nodes[1].Connectors[0].HomeDevice = NetworkManager.HomeDevices[2];
            NetworkManager.Nodes[1].Connectors[1].HomeDevice = NetworkManager.HomeDevices[3];
            //Puke: bidirectional
            NetworkManager.Nodes[1].Connectors[0].HomeDevice.Connector = NetworkManager.Nodes[1].Connectors[0];
            NetworkManager.Nodes[1].Connectors[1].HomeDevice.Connector = NetworkManager.Nodes[1].Connectors[1];
            NetworkManager.Nodes[1].Address = 0x4004;

            //Actions 0->1
            NetworkManager.Nodes[0].Connectors[0].HomeDevice.Operations.Add(new Operation()
            {
                OPCode = SmartHome.Comunications.OPCode.DigitalSwitch,
                Args = new byte[] { 0x03, 0x00 }, //Harcondig: IDHomeDev, Time = 0
                DestionationHomeDevice = NetworkManager.Nodes[1].Connectors[1].HomeDevice,
            });

            //1->0
            NetworkManager.Nodes[1].Connectors[0].HomeDevice.Operations.Add(new Operation()
            {
                OPCode = SmartHome.Comunications.OPCode.DigitalSwitch,
                Args = new byte[] { 0x01, 0x00 }, //Hardconding: IDHomeDev, Time = 0
                DestionationHomeDevice = NetworkManager.Nodes[0].Connectors[1].HomeDevice,
            });

            NetworkManager.GetAllEEPROMS();
        }
    }
}
