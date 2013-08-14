#region Using Statements
using AutoMapper;
using DataLayer;
using DataLayer.Entities.HomeDevices;
using ServiceLayer.DTO;
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
            Type deviceType = typeof(HomeDevice).Assembly.GetTypes().First(t => t.Name == homeDeviceType);

            HomeDevice homeDevice = (HomeDevice)Activator.CreateInstance(deviceType);

            homeDevice.Name = nameHomeDevice;

            Repositories.HomeDeviceRespository.Insert(homeDevice);

            var aa = Repositories.HomeDeviceRespository.GetAll();
            var aaaa = aa.Last();

            return Repositories.HomeDeviceRespository.GetAll().AsEnumerable().Last().Id;
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
            throw new NotImplementedException();
            //HomeDevice homeDevice = GET HOME DEVICE BY ID

            //if (homeDevice.Connector != null)
            //{
            //    Services.NodeService.UnlinkHomeDevice(idHomeDevice);
            //}

            //DELETE HOME DEVICE BY ID
        }

        /// <summary>
        /// Change the name of a one HomeDevice
        /// </summary>
        /// <param name="idHomeDevice">Identificator of the HomeDevice</param>
        /// <param name="newName">New Name</param>
        public void SetNameHomeDevice(int idHomeDevice, string newName)
        {
            throw new NotImplementedException();
            //NetworkManager.HomeDevices.FirstOrDefault(hd => hd.Id == idHomeDevice).Name = newName;
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
            throw new NotImplementedException();
        }

        public LocationDTO[] GetHomeDevicePosition(int idHomeDevice)
        {
            throw new NotImplementedException();
            //return Mapper.Map<LocationDTO>(NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice).Position);
        }

        /// <summary>
        /// Return all HomeDevices of the system (be or not be connected to a Node)
        /// </summary>
        /// <returns>Return a HomeDeviceDTO</returns>
        public IEnumerable<HomeDeviceDTO> GetHomeDevices()
        {
            var homeDevices = Repositories.HomeDeviceRespository.GetAll();

            return Mapper.Map<IEnumerable<HomeDeviceDTO>>(homeDevices);
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
        /// <param name="idLocation"></param>
        /// <returns></returns>
        public IEnumerable<HomeDeviceDTO> GetHomeDevices(int idLocation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone and type
        /// </summary>
        /// <param name="idLocation">Identificator of the localization</param>
        /// <param name="homeDeviceType">List of types of the HomeDevices</param>
        /// <returns>Return Dictionary with ID, Name and type</returns>
        public IEnumerable<HomeDeviceDTO> GetHomeDevices(int idLocation, List<string> homeDeviceTypes)
        {
            return GetHomeDevices(idLocation).Where(hd => homeDeviceTypes.Contains(hd.Type));
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone, a list of types and if are connected or not.
        /// </summary>
        /// <param name="idLocation"></param>
        /// <param name="homeDeviceTypes"></param>
        /// <param name="connected"></param>
        /// <returns></returns>
        public IEnumerable<HomeDeviceDTO> GetHomeDevices(int idLocation, List<string> homeDeviceTypes, bool connected)
        {
            return GetHomeDevices(idLocation, homeDeviceTypes).Where(hd => hd.InUse == connected);
        }

    }
}
