using System;

namespace DomoticNetwork.NetworkModel
{
    class Node
    {
        public UInt16 NodeAddress { set; get; }
        public String Name { set; get; }

        public SecurityNetwork Security { set; get; }

        //Position
        public Position NodePosition { set; get; }
        public bool Movil { set; get; }

        //Shield
        public Shield NodeShield { set; get; }

        public HomeDevice[] HomeDevices { set; get; }

        public Node(String name, bool movil, Shield.ShieldType shieldtype, Base.UControllerType basetype)
        {
            NodeAddress = 0x00; //TODO: Calcular
            Name = name;
            Movil = movil;
            if (movil)
            {
                CalculatePosition();
            }
            else
            {
                NodePosition = new Position() { Floor = "", X = 0, Y = 0 };
            }

            NodeShield = new Shield(shieldtype, basetype);

        }
        
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
        public int X { set; get; }
        public int Y { set; get; }
        public string Floor { set; get; }
    }
}
