using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomoticNetwork.NetworkModel
{
    class Network
    {
        public SecurityNetwork Security { set; get; }
        public List<Node> Nodes { set; get; }

        public Node GetNode(UInt16 Address)
        {
            return Nodes.First<Node>(x => x.NodeAddress == Address);
        }

        public void AddNode()
        {
            Nodes.Add(new Node("example", false, Shield.ShieldType.Roseta, Base.UControllerType.ATMega128RFA1));
        }

        public void UpdateNetwork(UInt16 direction)
        {
            //Algoritmo para generar la EEPROM de este nodoB
        }
    }

    class SecurityNetwork
    {
        public const Byte CHANNEL = 0x00;
        public const UInt16 PANID = 0x00;

        public Byte Channel { set; get; }
        public UInt16 PanId { set; get; }
        public String SecurityKey { set; get; }
    }
}
