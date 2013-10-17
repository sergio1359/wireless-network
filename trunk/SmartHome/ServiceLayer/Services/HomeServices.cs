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
using System.Collections.Generic;
#endregion

namespace ServiceLayer
{
    public class HomeService
    {
        #region GeneralHome

        public void SetHomeName(string newName)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Home home = repository.HomeRespository.GetHome();
                home.Name = newName;

                repository.Commit();
            }
        }

        public string GetHomeName()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                return repository.HomeRespository.GetHome().Name;
            }
        }

        public void SetHomeLocation(float latitude, float longitude)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Home home = repository.HomeRespository.GetHome();
                home.Location = new Coordenate() 
                { 
                    Latitude = latitude, 
                    Longitude = longitude 
                };

                repository.Commit();
            }
        }

        public Coordenate GetHomeLocation()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                return repository.HomeRespository.GetHome().Location;
            }
        }

        public void UnlinkNode(int idNode)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Node node = repository.NodeRespository.GetById(idNode);

                if (node == null)
                    return;

                node.UnlinkAllConnectors();//CHECK: works??

                repository.NodeRespository.Delete(node);

                repository.Commit();
            }
        }

        public void ForceUpdateNodeConfiguration(int idNode)
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

        public IEnumerable<ZoneDTO> GetZones()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var zones = repository.ZoneRepository.GetAll();

                return Mapper.Map<IEnumerable<ZoneDTO>>(zones);
            }
        }

        /// <summary>
        /// Add a Zone at Home and return it id.
        /// </summary>
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

        public void RemoveZone(int idZone) //CHECK method
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
        public IEnumerable<ZoneDTO> GetViews(int idZone)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = repository.ZoneRepository.GetById(idZone);

                if (zone == null)
                    return null;

                return Mapper.Map<IEnumerable<ZoneDTO>>(zone.Views);
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
        /// Add a View in a concrete Zone at Home, return the View's id
        /// </summary>
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
