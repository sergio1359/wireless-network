#region Using Statement
using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using ServiceLayer.DTO;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using SmartHome.BusinessEntities;
using SmartHome.Comunications;
using SmartHome.Communications.Modules.Config;
#endregion

namespace ServiceLayer
{
    public class HomeService
    {
        #region GeneralHome
        /// <summary>
        /// Change the name of the Home
        /// </summary>
        /// <param name="newName"></param>
        public void SetHomeName(string newName)
        {
            var home = Repositories.HomeRespository.GetHome();
            home.Name = newName;

            Repositories.SaveChanges();
        }

        /// <summary>
        /// Get the name of the Home
        /// </summary>
        /// <returns></returns>
        public string GetHomeName()
        {
            return Repositories.HomeRespository.GetHome().Name;
        }

        /// <summary>
        /// Define the location of a Home
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void SetHomeLocation(float latitude, float longitude)
        {
            Home home = Repositories.HomeRespository.GetHome();
            home.Location = new Coordenate() { Latitude = latitude, Longitude = longitude };

            Repositories.SaveChanges();
        }

        /// <summary>
        /// Return the location of the home
        /// </summary>
        /// <returns></returns>
        public Coordenate GetHomeLocation()
        {
            return Repositories.HomeRespository.GetHome().Location;
        }

        /// <summary>
        /// Unlink Node of the system.
        /// </summary>
        /// <param name="idNode"></param>
        public void UnlinkNode(int idNode)
        {
            Node node = Repositories.NodeRespository.GetById(idNode);

            if (node == null)
                return;

            node.UnlinkAllConnectors();

            Repositories.NodeRespository.Delete(node);
        }

        /// <summary>
        /// Force UpdateConfiguration
        /// </summary>
        public void UpdateConfiguration(int idNode)
        {
            Node node = Repositories.NodeRespository.GetById(idNode);

            if(node == null)
                return;

            CommunicationManager.Instance.FindModule<ConfigModule>().SendConfiguration(node);
        }
        #endregion

        #region Zones
        /// <summary>
        /// Return all the Zones
        /// </summary>
        /// <returns>Dictionary ID, Name of zones</returns>
        public ZoneDTO[] GetZones()
        {
            var zones = Repositories.ZoneRepository.GetAll();
            return Mapper.Map<ZoneDTO[]>(zones);
        }

        /// <summary>
        /// Add a zone in the system
        /// </summary>
        /// <param name="zone"></param>
        /// <returns>Devuelve el Id de la zona añadida</returns>
        public int AddZone(string nameZone)
        {
            Zone zone = new Zone()
            {
                Name = nameZone,
                Home = Repositories.HomeRespository.GetHome(),
            };

            zone = Repositories.ZoneRepository.Insert(zone);

            View view = new View()
            {
                Name = nameZone,
                Zone = zone
            };

            Repositories.ViewRepository.Insert(view);

            zone.MainView = view;

            Repositories.SaveChanges();

            return zone.Id;
        }

        /// <summary>
        /// Remove a Zone at Home
        /// </summary>
        /// <param name="idZone"></param>
        public void RemoveZone(int idZone)
        {
            //NO ES UN METODO SENCILLO DE PROGRAMAR
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change the name of the Zone
        /// </summary>
        /// <param name="idView"></param>
        /// <param name="newName"></param>
        public void SetNameZone(int idZone, string newName)
        {
            Zone zone = Repositories.ZoneRepository.GetById(idZone);

            if (zone == null)
                return;

            zone.Name = newName;
            zone.MainView.Name = newName;

            Repositories.SaveChanges();
        }

        #endregion

        #region Views
        public ViewDTO[] GetViews(int idZone)
        {
            Zone zone = Repositories.ZoneRepository.GetById(idZone);

            if (zone == null)
                return null;

            return Mapper.Map<ViewDTO[]>(zone.Views);
        }

        public byte[] GetViewImage(int idView)
        {
            View view = Repositories.ViewRepository.GetById(idView);

            if (view == null)
                return null;

            return view.ImageMap;
        }

        public void SetViewImage(int idView, byte[] newImage)
        {
            View view = Repositories.ViewRepository.GetById(idView);

            if (view == null)
                return;

            view.ImageMap = newImage;

            Repositories.SaveChanges();
        }

        public string GetNameView(int idView)
        {
            View view = Repositories.ViewRepository.GetById(idView);

            if (view == null)
                return null;

            return view.Name;
        }

        /// <summary>
        /// Change the name of the View
        /// </summary>
        /// <param name="idView"></param>
        /// <param name="newName"></param>
        public void SetNameView(int idView, string newName)
        {
            View view = Repositories.ViewRepository.GetById(idView);

            if (view == null)
                return;

            if (view.Id == view.Zone.MainView.Id)
                view.Zone.Name = newName;

            view.Name = newName;

            Repositories.SaveChanges();
        }

        /// <summary>
        /// Add a View in a concrete Zone at Home
        /// </summary>
        /// <param name="idZone">Identification of the Zone</param>
        /// <param name="nameView">Name of the View</param>
        /// <returns>Return the identification of the new View</returns>
        public int AddView(int idZone, string nameView)
        {
            Zone zone = Repositories.ZoneRepository.GetById(idZone);

            if (zone == null)
                return -1;

            View view = new View()
            {
                Name = nameView,
                Zone = zone,
            };
            view = Repositories.ViewRepository.Insert(view);

            zone.Views.Add(view);

            Repositories.SaveChanges();

            return view.Id;
        }

        /// <summary>
        /// Remove a concrete View
        /// </summary>
        /// <param name="idView"></param>
        public void RemoveView(int idView)
        {
            View view = Repositories.ViewRepository.GetById(idView);

            if (view != null)
                Repositories.ViewRepository.Delete(view);
        }

        #endregion
    }
}
