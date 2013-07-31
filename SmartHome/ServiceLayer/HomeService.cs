using SmartHome.HomeModel;
using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class HomeService
    {
        public void SetHomeName(string newName)
        {
            NetworkManager.Home.Name = newName;
        }

        public string GetHomeName()
        {
            return NetworkManager.Home.Name;
        }

        public void SetHomeLocation(float latitude, float longitude)
        {
            NetworkManager.Home.Location = new Coordenate() { Latitude = latitude, Longitude = longitude };
        }

        public Coordenate GetHomeLocation()
        {
            return NetworkManager.Home.Location;
        }

        public uint[] GetPendingNodes()
        {
            throw new NotImplementedException();
        }

        public void AllowPendingNode(string MAC)
        {
        }

        public void DenyPendingNode(string MAC)
        {

        }

        public void UnlinkNode(int idNode)
        {
            Node node = NetworkManager.Nodes.FirstOrDefault(n => n.Id == idNode);
            NetworkManager.Nodes.Remove(node);
        }

        public Dictionary<int, string> GetZones()
        {
            return NetworkManager.Home.Zones.ToDictionary(z => z.Id , z=> z.NameZone);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zone"></param>
        /// <returns>Devuelve el Id de la zona añadida</returns>
        public int AddZone(Zone zone)
        {
            NetworkManager.Home.Zones.Add(zone);
            return NetworkManager.Home.Zones.Last().Id;
        }

        public void SetNameZone(int idZone, string newName)
        {
            NetworkManager.Home.Zones.FirstOrDefault(z => z.Id == idZone).NameZone= newName;
        }

        public void UpdateConfiguration()
        {

        }

        
    }
}
