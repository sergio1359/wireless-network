using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomoticNetwork.NetworkModel
{
    class Network
    {
        public Byte Channel { set; get; }
        public UInt16 PanId { set; get; }
        public Byte[] SecurityKey { set; get; }

        List<Node> Nodes { set; get; }

        public void GenerateKey(String key)
        {
        }

        public Node GetNode(UInt16 Address)
        {
            return Nodes.First<Node>(x => x.NodeAddress == Address);
        }


        public void GenerateEEPROM(UInt16 direction)
        {
            //Algoritmo para generar la EEPROM de este nodoB
        }
    }
}
