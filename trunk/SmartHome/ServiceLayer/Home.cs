using SmartHome.HomeModel;
using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class HomeService
    {

        /// <summary>
        /// Change the name of the Home
        /// </summary>
        /// <param name="newName"></param>
        public static void SetHomeName(string newName)
        {
            NetworkManager.Home.Name = newName;
        }

        /// <summary>
        /// Get the name of the Home
        /// </summary>
        /// <returns></returns>
        public static string GetHomeName()
        {
            return NetworkManager.Home.Name;
        }

        /// <summary>
        /// Define the location of a Home
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public static void SetHomeLocation(float latitude, float longitude)
        {
            NetworkManager.Home.Location = new Coordenate() { Latitude = latitude, Longitude = longitude };
        }

        /// <summary>
        /// Return the location of the home
        /// </summary>
        /// <returns></returns>
        public static Coordenate GetHomeLocation()
        {
            return NetworkManager.Home.Location;
        }

        /// <summary>
        /// Return the MAC of the Pending Nodes
        /// </summary>
        /// <returns></returns>
        public static uint[] GetPendingNodes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Allow a MAC in the system.
        /// </summary>
        /// <param name="MAC"></param>
        public static void AllowPendingNode(string MAC)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deny a MAC in the system
        /// </summary>
        /// <param name="MAC"></param>
        public static void DenyPendingNode(string MAC)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unlink Node of the system.
        /// </summary>
        /// <param name="idNode"></param>
        public static void UnlinkNode(int idNode)
        {
            Node node = NetworkManager.Nodes.FirstOrDefault(n => n.Id == idNode);
            NetworkManager.Nodes.Remove(node);
        }

        /// <summary>
        /// Return all the zones
        /// </summary>
        /// <returns>Dictionary ID, Name of zones</returns>
        public static Dictionary<int, string> GetZones()
        {
            return NetworkManager.Home.Zones.ToDictionary(z => z.Id , z=> z.NameZone);
        }

        /// <summary>
        /// Return all the view of a zone
        /// </summary>
        /// <param name="idZone"></param>
        /// <returns></returns>
        public static Dictionary<int, string> GetViews(int idZone)
        {
            return NetworkManager.Home.Zones.First(z => z.Id == idZone).Views.ToDictionary(v => v.Id, v => v.NameView);
        }

        /// <summary>
        /// Add a zone in the system
        /// </summary>
        /// <param name="zone"></param>
        /// <returns>Devuelve el Id de la zona añadida</returns>
        public static int AddZone(string nameZone)
        {
            Zone zone = new Zone();
            zone.NameZone = nameZone;
            NetworkManager.Home.Zones.Add(zone);

            return NetworkManager.Home.Zones.Last().Id;
        }

        public static int AddView(int idZone, string nameView)
        {
            var zone = NetworkManager.Home.Zones.First(z => z.Id == idZone);
            View newView = new View();
            newView.NameView = nameView;
            zone.Views.Add(newView);

            return newView.Id;
        }

        public static Image GetImageZone(int idZone)
        {
            throw new NotImplementedException();
        }

        public static Image GetImageView(int idView)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the name zone
        /// </summary>
        /// <param name="idZone"></param>
        /// <param name="newName"></param>
        public static void SetNameZone(int idZone, string newName)
        {
            NetworkManager.Home.Zones.First(z => z.Id == idZone).NameZone= newName;
        }

        public static void SetNameView(int idView, string newName)
        {
            NetworkManager.Home.Zones.SelectMany(z => z.Views).First(v => v.Id == idView).NameView = newName;
        }

        /// <summary>
        /// Update the EEPROMs of all the nodes
        /// </summary>
        public static void UpdateConfiguration()
        {
            NetworkManager.GetAllEEPROMS();
        }

        
    }
}
