using SmartHome.HomeModel;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network
{
    public static class NetworkManager
    {
        public static Security Security = new Security();
        public static List<Node> Nodes = new List<Node>();

        public static List<HomeDevice> HomeDevices = new List<HomeDevice>();

        public static Home Home = new Home();


        //TODO: esto es solo para el ejemplo que estamos haciendo
        public static void GetAllEEPROMS()
        {
            Nodes.ForEach(n => n.GetEEPROM());
        }
    }
}
