#region Using Statement
using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using ServiceLayer.DTO;
using System;
using System.Drawing;
using System.Linq;
#endregion

namespace ServiceLayer
{
    public class HomeService
    {
        #region GeneralHome

        public void CreateHome()
        {
            if (!Repositories.HomeRespository.IsHomeCreated())
            {
                Home home = new Home();

                Repositories.HomeRespository.Insert(home);
            }
        }

        public bool ExitsHome()
        {
            return Repositories.HomeRespository.IsHomeCreated();
        }

        /// <summary>
        /// Change the name of the Home
        /// </summary>
        /// <param name="newName"></param>
        public void SetHomeName(string newName)
        {
            if (Repositories.HomeRespository.IsHomeCreated())
            {
                var home = Repositories.HomeRespository.GetHome();
                home.Name = newName;

                Repositories.SaveChanges();
            }
        }

        /// <summary>
        /// Get the name of the Home
        /// </summary>
        /// <returns></returns>
        public string GetHomeName()
        {
            if (Repositories.HomeRespository.IsHomeCreated())
                return Repositories.HomeRespository.GetHome().Name;

            return "The Home had not been created";
        }

        /// <summary>
        /// Define the location of a Home
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void SetHomeLocation(float latitude, float longitude)
        {
            if (Repositories.HomeRespository.IsHomeCreated())
            {
                Home home = Repositories.HomeRespository.GetHome();
                home.Location = new Coordenate() { Latitude = latitude, Longitude = longitude };

                Repositories.SaveChanges();
            }
        }

        /// <summary>
        /// Return the location of the home
        /// </summary>
        /// <returns></returns>
        public Coordenate GetHomeLocation()
        {
            if (Repositories.HomeRespository.IsHomeCreated())
                return Repositories.HomeRespository.GetHome().Location;

            return null;
        }

        /// <summary>
        /// Return the MAC of the Pending Nodes
        /// </summary>
        /// <returns>Return string for the MACs</returns>
        public string[] GetPendingNodes()
        {
            throw new NotImplementedException();
            //VICTOR
        }

        /// <summary>
        /// Allow a MAC in the system.
        /// </summary>
        /// <param name="MAC"></param>
        public void AllowPendingNode(string MAC)
        {
            throw new NotImplementedException();
            //VICTOR
        }

        /// <summary>
        /// Deny a MAC in the system
        /// </summary>
        /// <param name="MAC"></param>
        public void DenyPendingNode(string MAC)
        {
            throw new NotImplementedException();
            //VICTOR
        }

        /// <summary>
        /// Unlink Node of the system.
        /// </summary>
        /// <param name="idNode"></param>
        public void UnlinkNode(int idNode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the EEPROMs of all the nodes
        /// </summary>
        public void UpdateConfiguration()
        {
            throw new NotImplementedException();
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
            throw new NotFiniteNumberException();
            Zone zone = new Zone();
            zone.Name = nameZone;
            Repositories.ZoneRepository.Insert(zone);

            //return NetworkManager.Home.Zones.Last().Id;
        }

        /// <summary>
        /// Remove a Zone at Home
        /// </summary>
        /// <param name="idZone"></param>
        public void RemoveZone(int idZone)
        {
            //Borrar todos los views que contenga la zona
        }

        /// <summary>
        /// Change the name of the Zone
        /// </summary>
        /// <param name="idView"></param>
        /// <param name="newName"></param>
        public void SetNameZone(int idView, string newName)
        {
            //NetworkManager.Home.Zones.SelectMany(z => z.Views).First(v => v.Id == idView).NameView = newName;
        } 

        /// <summary>
        /// Add a View in a concrete Zone at Home
        /// </summary>
        /// <param name="idZone">Identification of the Zone</param>
        /// <param name="nameView">Name of the View</param>
        /// <returns>Return the identification of the new View</returns>
        public int AddView(int idZone, string nameView)
        {
            throw new NotImplementedException();
            //var zone = NetworkManager.Home.Zones.First(z => z.Id == idZone);
            //View newView = new View();
            //newView.Name = nameView;
            //zone.Views.Add(newView);

            //return newView.Id;
        }

        /// <summary>
        /// Remove a concrete View
        /// </summary>
        /// <param name="idView"></param>
        public void RemoveView(int idView)
        {
            //detalles de borrar un view concreto
        }

        /// <summary>
        /// Change the name of the View
        /// </summary>
        /// <param name="idView"></param>
        /// <param name="newName"></param>
        public void SetNameView(int idView, string newName)
        {
            //NetworkManager.Home.Zones.SelectMany(z => z.Views).First(v => v.Id == idView).NameView = newName;
        }

        #endregion
    }
}
