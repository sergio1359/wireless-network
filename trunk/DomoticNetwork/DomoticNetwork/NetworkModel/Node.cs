using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomoticNetwork.NetworkModel
{
    class Node
    {
        public UInt16 NodeAddress { set; get; }
        public String ID { set; get; }

        //Security information in node
        SecurityNetwork SecureNode { set; get; }

        //Position
        public Position NodePosition { set; get; }
        public bool Movil { set; get; }

        //Shield
        public Shield NodeShield { set; get; }

        public HomeDevice[] HomeDevices { set; get; }
        
        public void CalculatePosition()
        {
            if (Movil)
            {
                //Algoritmo de posicionamiento por triangulacion
            }
        }
    }

    class Position 
    {
        public int x { set; get; }
        public int y { set; get; }
        public string floor { set; get; }
    }
}
