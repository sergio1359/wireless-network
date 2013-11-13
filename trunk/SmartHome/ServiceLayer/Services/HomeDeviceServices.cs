#region Using Statements
using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using ServiceLayer.DTO;
using SmartHome.BusinessEntities;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace ServiceLayer
{
    public class HomeDeviceService
    {
        public int AddHomeDevice(string nameHomeDevice, string homeDeviceType)
        {
            HomeDevice homeDevice;
            try
            {
                homeDevice = BusinessHomeDevice.CreateHomeDevice(homeDeviceType);
            }
            catch (Exception)
            {
                throw new ArgumentException("Home Device " + homeDeviceType + " unsupported");
            }

            homeDevice.Name = nameHomeDevice;

            UnitOfWork repository = UnitOfWork.GetInstance();

            homeDevice = repository.HomeDeviceRespository.Insert(homeDevice);
            repository.Commit();

            return homeDevice.Id;
        }

        public string[] GetHomeDeviceTypes()
        {
            return HomeDevice.HomeDeviceTypes
                .Select(t => t.Name)
                .ToArray();
        }

        public void RemoveHomeDevice(int idHomeDevice)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

            if (homeDevice == null)
                throw new ArgumentException("Home Device id doesn't exist");

            if (homeDevice.InUse)
            {
                //UPDATE CHECKSUM
                homeDevice.Connector.Node.UpdateChecksum(null);

                Services.NodeService.UnlinkHomeDevice(idHomeDevice);
            }

            repository.HomeDeviceRespository.Delete(homeDevice);

            repository.Commit();
        }

        public void SetNameHomeDevice(int idHomeDevice, string newName)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

            if (homeDevice == null)
                throw new ArgumentException("Home device id doesn't exist");

            homeDevice.Name = newName;

            repository.Commit();
        }

        /// <param name="x">Relative position 0 to 1 of the X axis</param>
        /// <param name="y">Relative position 0 to 1 of the Y axis</param>
        public void UpdateLocation(int idLocation, float x, float y)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Location location = repository.LocationRepository.GetById(idLocation);

            if (location == null)
                throw new ArgumentException("Location id doesn't exist");

            location.X = x;
            location.Y = y;

            repository.Commit();
        }

        public IEnumerable<LocationDTO> GetHomeDeviceLocations(int idHomeDevice)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            var homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

            if (homeDevice == null)
                throw new ArgumentException("Home device id doesn't exist");

            return Mapper.Map<IEnumerable<LocationDTO>>(homeDevice.Locations);
        }

        public IEnumerable<HomeDeviceDTO> GetAllHomeDevices()
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            var homeDevices = repository.HomeDeviceRespository.GetAll();

            return Mapper.Map<IEnumerable<HomeDeviceDTO>>(homeDevices);
        }

        public IEnumerable<HomeDeviceDTO> GetAllHomeDevices(bool inUse)
        {
            return GetAllHomeDevices().Where(hd => hd.InUse == inUse);
        }

        public IEnumerable<HomeDeviceDTO> GetAllHomeDevicesInAView(int idView)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            if (repository.ViewRepository.GetById(idView) == null)
                throw new ArgumentException("View id doesn't exist");

            var homeDevices = repository.HomeDeviceRespository.GetHomeDevicesWithLocations()
                .Where(hd => hd.Locations.Any(l => l.View.Id == idView));

            return Mapper.Map<IEnumerable<HomeDeviceDTO>>(homeDevices);
        }

        public IEnumerable<HomeDeviceDTO> GetHomeDevicesInAView(int idView, List<string> homeDeviceTypes)
        {
            return GetAllHomeDevicesInAView(idView)
                .Where(hd => homeDeviceTypes.Contains(hd.Type));
        }

        public string[] GetNameProducts()
        {
            return BusinessProduct.GetProducts.Select(p => p.Name)
                .ToArray();
        }
    }
}
