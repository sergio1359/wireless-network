using SmartHome.HomeModel;
using SmartHome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class HomeService
    {
        public void SetHomeName(string name)
        {
            NetworkManager.Home.Name = name;
        }

        public string GetHomeName()
        {
            return NetworkManager.Home.Name;
        }

        public void SetHomeCity(double latitude, double longitude)
        {
            NetworkManager.Home.Latitude = latitude;
            NetworkManager.Home.Longitude = longitude;
        }

        public Tuple<double, double> GetHomeCity()
        {
            return new Tuple<double, double>(NetworkManager.Home.Latitude, NetworkManager.Home.Longitude);
        }

        public uint GetPendingNodes()
        {
            throw new NotImplementedException();
        }

        public void UnlinkNode(uint MAC)
        {
            Node node = NetworkManager.Nodes.FirstOrDefault(n => n.Mac == MAC);
            NetworkManager.Nodes.Remove(node);
        }

        public string[] GetZones()
        {
            return NetworkManager.Home.Zones.Select(z => z.NameZone).ToArray();
        }

        public Zone GetZone(string zone)
        {
            return NetworkManager.Home.Zones.FirstOrDefault(z => z.NameZone == zone);
        }

        public void AddZone(string zone)
        {
            NetworkManager.Home.Zones.Add(new Zone() { NameZone = zone });
        }

        public void SetNameZone(string zone, string newName)
        {
            NetworkManager.Home.Zones.FirstOrDefault(z => z.NameZone == zone).NameZone = newName;
        }
    }
}
