using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomoticNetwork.NetworkModel
{
    class Network
    {
        SecurityNetwork Security { set; get; }
        List<Node> Nodes { set; get; }

        public Node GetNode(UInt16 Address)
        {
            return Nodes.First<Node>(x => x.NodeAddress == Address);
        }


        public void UpdateNetwork(UInt16 direction)
        {
            //Algoritmo para generar la EEPROM de este nodoB
        }
    }

    class SecurityNetwork
    {
        public Byte Channel { set; get; }
        public UInt16 PanId { set; get; }
        public String SecurityKey { set; get; }
    }
}
