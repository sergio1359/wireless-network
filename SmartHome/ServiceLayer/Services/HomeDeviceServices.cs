using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using DataLayer.Repositories;
using ServiceLayer.DTO;
using AutoMapper;
using System.Reflection;
using DataLayer.Entities.HomeDevices;
using DataLayer.Entities;

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
            throw new NotImplementedException();
            //Type deviceType = typeof(HomeDevice).Assembly.GetTypes().First(t => t.Name == homeDeviceType);

            //HomeDevice homeDevice = (HomeDevice)Activator.CreateInstance(deviceType);

            //homeDevice.Name = nameHomeDevice;

            //NetworkManager.HomeDevices.Add(homeDevice);

            //return NetworkManager.HomeDevices.Last().Id;
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
            //HomeDevice homeDevice = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);

            //if (homeDevice.Connector != null)
            //{
            //    Services.NodeService.UnlinkHomeDevice(idHomeDevice);
            //}

            //NetworkManager.HomeDevices.Remove(homeDevice);
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
        /// <param name="homeDevice">Identificator of the home device to be move</param>
        /// <param name="Zone">new zone</param>
        /// <param name="x">Relative position 0 to 1 of the X axis</param>
        /// <param name="y">Relative position 0 to 1 of the X axis</param>
        public void UpdateZonePosition(int idHomeDevice, int idZone, float x, float y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the position of a HomeDevice in a View
        /// </summary>
        /// <param name="homeDevice">Identificator of the home device to be move</param>
        /// <param name="Zone">new zone</param>
        /// <param name="x">Relative position 0 to 1 of the X axis</param>
        /// <param name="y">Relative position 0 to 1 of the X axis</param>
        public void UpdateViewPosition(int idHomeDevice, int idView, float x, float y)
        {
            throw new NotImplementedException();
        }

        public LocationDTO GetHomeDevicePosition(int idHomeDevice)
        {
            throw new NotImplementedException();
            //return Mapper.Map<LocationDTO>(NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice).Position);
        }

        /// <summary>
        /// Return all HomeDevices of the system (be or not be connected to a Node)
        /// </summary>
        /// <returns>Return a HomeDeviceDTO</returns>
        public HomeDeviceDTO[] GetHomeDevices()
        {
            throw new NotImplementedException();
            //return Mapper.Map<List<HomeDeviceDTO>>(NetworkManager.HomeDevices).ToArray();
        }

        /// <summary>
        /// Return all HomeDevices of the system (be or not be connected to a Node)
        /// </summary>
        /// <returns>Return a HomeDeviceDTO</returns>
        public HomeDeviceDTO[] GetHomeDevices(bool isInUse)
        {

            throw new NotImplementedException();
            //var homeDevices = NetworkManager.HomeDevices.Where(hd => hd.InUse == isInUse);

            //return Mapper.Map<List<HomeDeviceDTO>>(homeDevices).ToArray();
        }

        public HomeDeviceDTO[] GetHomeDevicesByZone(int idZona)
        {
            throw new NotImplementedException();
            //var homeDevices = NetworkManager.HomeDevices.Where(hd => hd.InUse == true && hd.Position.Zone.Id == idZona);

            //return Mapper.Map<List<HomeDeviceDTO>>(homeDevices).ToArray();
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone and type
        /// </summary>
        /// <param name="zona">Identificator of the zone</param>
        /// <param name="homeDeviceType">Identificator of the type</param>
        /// <returns>Return Dictionary with ID, Name and type</returns>
        public HomeDeviceDTO[] GetHomeDevicesByZone(int idZona, List<string> homeDeviceTypes)
        {
            throw new NotImplementedException();
            //var homeDevices = NetworkManager.HomeDevices.Where(hd => hd.InUse == true && hd.Position.Zone.Id == idZona && homeDeviceTypes.Contains(hd.HomeDeviceType));

            //return Mapper.Map<List<HomeDeviceDTO>>(homeDevices).ToArray();
        }

        public HomeDeviceDTO[] GetHomeDevicesByView(int idView)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone and type
        /// </summary>
        /// <param name="zona">Identificator of the zone</param>
        /// <param name="homeDeviceType">Identificator of the type</param>
        /// <returns>Return Dictionary with ID, Name and type</returns>
        public HomeDeviceDTO[] GetHomeDevicesByView(int idZona, List<string> homeDeviceTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the HomeDevices connected of a concrete zone, a list of types and if are connected or not.
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="homeDeviceTypes"></param>
        /// <param name="connected"></param>
        /// <returns></returns>
        public HomeDeviceDTO[] GetHomeDevices(int idZona, List<string> homeDeviceTypes, bool connected)
        {
            throw new NotImplementedException();
            //var homeDevices = NetworkManager.HomeDevices.Where(hd => hd.Position.Zone.Id == idZona && homeDeviceTypes.Contains(hd.HomeDeviceType) && hd.InUse == connected);

            //return Mapper.Map<List<HomeDeviceDTO>>(homeDevices).ToArray();
        }

    }
}
