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
using SmartHome.Communications;
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
            using (UnitOfWork repository = new UnitOfWork())
            {
                Home home = repository.HomeRespository.GetHome();
                home.Name = newName;

                repository.Commit();
            }
        }

        /// <summary>
        /// Get the name of the Home
        /// </summary>
        /// <returns></returns>
        public string GetHomeName()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                return repository.HomeRespository.GetHome().Name;
            }
        }

        /// <summary>
        /// Define the location of a Home
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void SetHomeLocation(float latitude, float longitude)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Home home = repository.HomeRespository.GetHome();
                home.Location = new Coordenate() { Latitude = latitude, Longitude = longitude };

                repository.Commit();
            }
        }

        /// <summary>
        /// Return the location of the home
        /// </summary>
        /// <returns></returns>
        public Coordenate GetHomeLocation()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                return repository.HomeRespository.GetHome().Location;
            }
        }

        /// <summary>
        /// Unlink Node of the system.
        /// </summary>
        /// <param name="idNode"></param>
        public void UnlinkNode(int idNode)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Node node = repository.NodeRespository.GetById(idNode);

                if (node == null)
                    return;

                node.UnlinkAllConnectors();

                //foreach (Connector connector in node.Connectors)
                //{
                //    repository.ConnectorRepository.Delete(connector);
                //}

                repository.NodeRespository.Delete(node);

                repository.Commit();
            }
        }

        /// <summary>
        /// Force UpdateConfiguration
        /// </summary>
        public void UpdateConfiguration(int idNode)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Node node = repository.NodeRespository.GetById(idNode);
                Home home = repository.HomeRespository.GetHome();

                if (node == null)
                    return;

                CommunicationManager.Instance.FindModule<ConfigModule>().SendConfiguration(node, home);   
            }         
        }
        #endregion

        #region Zones
        /// <summary>
        /// Return all the Zones
        /// </summary>
        /// <returns>Dictionary ID, Name of zones</returns>
        public ZoneDTO[] GetZones()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var zones = repository.ZoneRepository.GetAll();

                return Mapper.Map<ZoneDTO[]>(zones);
            }
        }

        /// <summary>
        /// Add a zone in the system
        /// </summary>
        /// <param name="zone"></param>
        /// <returns>Devuelve el Id de la zona añadida</returns>
        public int AddZone(string nameZone)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = new Zone()
                {
                    Name = nameZone,
                    Home = repository.HomeRespository.GetHome(),
                };

                zone = repository.ZoneRepository.Insert(zone);

                View view = new View()
                {
                    Name = nameZone,
                    Zone = zone
                };

                repository.ViewRepository.Insert(view);

                zone.MainView = view;

                repository.Commit();

                return zone.Id;
            }
        }

        /// <summary>
        /// Remove a Zone at Home CHECK
        /// </summary>
        /// <param name="idZone"></param>
        public void RemoveZone(int idZone)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = repository.ZoneRepository.GetById(idZone);

                repository.ViewRepository.Delete(zone.MainView);

                for (int i = 0; i < zone.Views.Count; i++)
                {
                    repository.ViewRepository.Delete(zone.Views.ElementAt(i));
                }

                repository.ZoneRepository.Delete(zone);

                repository.Commit();
            }
        }

        /// <summary>
        /// Change the name of the Zone
        /// </summary>
        /// <param name="idView"></param>
        /// <param name="newName"></param>
        public void SetNameZone(int idZone, string newName)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = repository.ZoneRepository.GetById(idZone);

                if (zone == null)
                    return;

                zone.Name = newName;
                zone.MainView.Name = newName;

                repository.Commit();
            }
        }

        #endregion

        #region Views
        public ViewDTO[] GetViews(int idZone)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = repository.ZoneRepository.GetById(idZone);

                if (zone == null)
                    return null;

                return Mapper.Map<ViewDTO[]>(zone.Views);
            }
        }

        public byte[] GetViewImage(int idView)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view == null)
                    return null;

                return view.ImageMap;
            }
        }

        public void SetViewImage(int idView, byte[] newImage)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view == null)
                    return;

                view.ImageMap = newImage;

                repository.Commit();
            }
        }

        public string GetNameView(int idView)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view == null)
                    return null;

                return view.Name;
            }
        }

        /// <summary>
        /// Change the name of the View
        /// </summary>
        /// <param name="idView"></param>
        /// <param name="newName"></param>
        public void SetNameView(int idView, string newName)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view == null)
                    return;

                if (view.Id == view.Zone.MainView.Id)
                    view.Zone.Name = newName;

                view.Name = newName;

                repository.Commit();
            }
        }

        /// <summary>
        /// Add a View in a concrete Zone at Home
        /// </summary>
        /// <param name="idZone">Identification of the Zone</param>
        /// <param name="nameView">Name of the View</param>
        /// <returns>Return the identification of the new View</returns>
        public int AddView(int idZone, string nameView)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = repository.ZoneRepository.GetById(idZone);

                if (zone == null)
                    return -1;

                View view = new View()
                {
                    Name = nameView,
                    Zone = zone,
                };
                view = repository.ViewRepository.Insert(view);

                zone.Views.Add(view);

                repository.Commit();

                return view.Id;
            }
        }

        /// <summary>
        /// Remove a concrete View
        /// </summary>
        /// <param name="idView"></param>
        public void RemoveView(int idView)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view != null)
                {
                    repository.ViewRepository.Delete(view);
                    repository.Commit();
                }
            }
        }

        #endregion
    }
}
