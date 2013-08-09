using AutoMapper;
using ServiceLayer.DTO;
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
        public void SetHomeName(string newName)
        {
            NetworkManager.Home.Name = newName;
        }

        /// <summary>
        /// Get the name of the Home
        /// </summary>
        /// <returns></returns>
        public string GetHomeName()
        {
            return NetworkManager.Home.Name;
        }

        /// <summary>
        /// Define the location of a Home
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void SetHomeLocation(float latitude, float longitude)
        {
            NetworkManager.Home.Location = new Coordenate() { Latitude = latitude, Longitude = longitude };
        }

        /// <summary>
        /// Return the location of the home
        /// </summary>
        /// <returns></returns>
        public Coordenate GetHomeLocation()
        {
            return NetworkManager.Home.Location;
        }

        /// <summary>
        /// Return the MAC of the Pending Nodes
        /// </summary>
        /// <returns>Return string for the MACs</returns>
        public string[] GetPendingNodes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Allow a MAC in the system.
        /// </summary>
        /// <param name="MAC"></param>
        public void AllowPendingNode(string MAC)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deny a MAC in the system
        /// </summary>
        /// <param name="MAC"></param>
        public void DenyPendingNode(string MAC)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unlink Node of the system.
        /// </summary>
        /// <param name="idNode"></param>
        public void UnlinkNode(int idNode)
        {
            Node node = NetworkManager.Nodes.FirstOrDefault(n => n.Id == idNode);
            NetworkManager.Nodes.Remove(node);
        }

        /// <summary>
        /// Return all the zones
        /// </summary>
        /// <returns>Dictionary ID, Name of zones</returns>
        public PlaceDTO[] GetZones()
        {
            return Mapper.Map<List<PlaceDTO>>(NetworkManager.Home.Zones).ToArray();
        }

        /// <summary>
        /// Return all the view of a zone
        /// </summary>
        /// <param name="idZone"></param>
        /// <returns></returns>
        public PlaceDTO[] GetViews(int idZone)
        {
            var views = NetworkManager.Home.Zones.First(z => z.Id == idZone);

            return Mapper.Map<List<PlaceDTO>>(views).ToArray();
        }

        /// <summary>
        /// Add a zone in the system
        /// </summary>
        /// <param name="zone"></param>
        /// <returns>Devuelve el Id de la zona añadida</returns>
        public int AddZone(string nameZone)
        {
            Zone zone = new Zone();
            zone.NameZone = nameZone;
            NetworkManager.Home.Zones.Add(zone);

            return NetworkManager.Home.Zones.Last().Id;
        }

        public int AddView(int idZone, string nameView)
        {
            var zone = NetworkManager.Home.Zones.First(z => z.Id == idZone);
            View newView = new View();
            newView.NameView = nameView;
            zone.Views.Add(newView);

            return newView.Id;
        }

        public Image GetImageZone(int idZone)
        {
            return NetworkManager.Home.Zones.First(z => z.Id == idZone).ImageMap;
        }

        public Image GetImageView(int idView)
        {
            return NetworkManager.Home.Zones.SelectMany(z => z.Views).First(v => v.Id == idView).ImageMap;
        }

        /// <summary>
        /// Set the name zone
        /// </summary>
        /// <param name="idZone"></param>
        /// <param name="newName"></param>
        public void SetNameZone(int idZone, string newName)
        {
            NetworkManager.Home.Zones.First(z => z.Id == idZone).NameZone= newName;
        }

        public void SetNameView(int idView, string newName)
        {
            NetworkManager.Home.Zones.SelectMany(z => z.Views).First(v => v.Id == idView).NameView = newName;
        }

        /// <summary>
        /// Update the EEPROMs of all the nodes
        /// </summary>
        public void UpdateConfiguration()
        {
            NetworkManager.GetAllEEPROMS();
        }
    }
}
