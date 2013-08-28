#region Using Statements
using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using ServiceLayer.DTO;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace ServiceLayer
{
    public class HomeDeviceService
    {
        /// <summary>
        /// Add a new HomeDevice to the system.
        /// </summary>
        /// <param name="nameHomeDevice">Name of the new HomeDevice</param>
        /// <param name="homeDeviceType">Type of HomeDevice</param>
        /// <returns>Return the ID for the new HomeDevice</returns>
        public int AddHomeDevice(string nameHomeDevice, string homeDeviceType)
        {
            HomeDevice homeDevice = BusinessHomeDevice.CreateHomeDevice(homeDeviceType);
            homeDevice.Name = nameHomeDevice;

            using (UnitOfWork repository = new UnitOfWork())
            {
                homeDevice = repository.HomeDeviceRespository.Insert(homeDevice);
                repository.Commit();
            }

            return homeDevice.Id;
        }

        /// <summary>
        /// Return the HomeDevice's types of the system.
        /// </summary>
        /// <returns>Array with the types names</returns>
        public string[] GetHomeDeviceTypes()
        {
            return HomeDevice.HomeDeviceTypes;
        }

        /// <summary>
        /// Remove, and unlink if is necesary, a HomeDevice of the system.
        /// </summary>
        /// <param name="idHomeDevice">Identificator of the HomeDevice to be remove.</param>
        public void RemoveHomeDevice(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    return;

                if (homeDevice.InUse)
                {
                    //UPDATE CHECKSUM
                    homeDevice.Connector.Node.ConfigChecksum = null;

                    Services.NodeService.UnlinkHomeDevice(idHomeDevice);
                }

                repository.HomeDeviceRespository.Delete(homeDevice);

                repository.Commit();
            }
        }

        /// <summary>
        /// Change the name of a one HomeDevice
        /// </summary>
        /// <param name="idHomeDevice">Identificator of the HomeDevice</param>
        /// <param name="newName">New Name</param>
        public void SetNameHomeDevice(int idHomeDevice, string newName)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    return;

                homeDevice.Name = newName;

                repository.Commit();
            }
        }

        /// <summary>
        /// Update the position of a HomeDevice in a Zone
        /// </summary>
        /// <param name="idHomeDevice">Identificator of the home device to be move</param>
        /// <param name="idView">new zone</param>
        /// <param name="x">Relative position 0 to 1 of the X axis</param>
        /// <param name="y">Relative position 0 to 1 of the X axis</param>
        public void UpdateLocation(int idLocation, float x, float y)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Location location = repository.LocationRepository.GetById(idLocation);

                if (location == null)
                    return;

                location.X = x;
                location.Y = y;

                repository.Commit();
            }
        }

        public LocationDTO[] GetHomeDeviceLocations(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    return null;

                return Mapper.Map<LocationDTO[]>(homeDevice.Location);
            }
        }

        /// <summary>
        /// Return all HomeDevices of the system (be or not be connected to a Node)
        /// </summary>
        /// <returns>Return a HomeDeviceDTO</returns>
        public IEnumerable<HomeDeviceDTO> GetHomeDevices()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var homeDevices = repository.HomeDeviceRespository.GetAll();

                return Mapper.Map<IEnumerable<HomeDeviceDTO>>(homeDevices);
            }
        }

        /// <summary>
        /// Return all HomeDevices specifying if are or not in use
        /// </summary>
        /// <param name="isInUse"></param>
        /// <returns></returns>
        public IEnumerable<HomeDeviceDTO> GetHomeDevices(bool isInUse)
        {
            return GetHomeDevices().Where(hd => hd.InUse == isInUse);
        }

        /// <summary>
        /// Return connected HomeDevice in a View of the of the system
        /// </summary>
        /// <param name="idView"></param>
        /// <returns></returns>
        public IEnumerable<HomeDeviceDTO> GetHomeDevices(int idView)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                if (repository.ViewRepository.GetById(idView) == null)
                    return null;

                var homeDevices = repository.HomeDeviceRespository.GetHomeDevicesWithLocations().Where(hd => hd.Location.Any(l => l.View.Id == idView));

                return Mapper.Map<IEnumerable<HomeDeviceDTO>>(homeDevices);
            }
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone and type
        /// </summary>
        /// <param name="idView">Identificator of the localization</param>
        /// <param name="homeDeviceType">List of types of the HomeDevices</param>
        /// <returns>Return Dictionary with ID, Name and type</returns>
        public IEnumerable<HomeDeviceDTO> GetHomeDevices(int idView, List<string> homeDeviceTypes)
        {
            return GetHomeDevices(idView).Where(hd => homeDeviceTypes.Contains(hd.Type));
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone, a list of types and if are connected or not.
        /// </summary>
        /// <param name="idView"></param>
        /// <param name="homeDeviceTypes"></param>
        /// <param name="connected"></param>
        /// <returns></returns>
        public IEnumerable<HomeDeviceDTO> GetHomeDevices(int idView, List<string> homeDeviceTypes, bool connected)
        {
            return GetHomeDevices(idView, homeDeviceTypes).Where(hd => hd.InUse == connected);
        }

    }
}
