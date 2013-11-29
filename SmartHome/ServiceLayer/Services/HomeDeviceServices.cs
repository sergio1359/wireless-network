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
using SmartHome.Communications.Modules;
#endregion

namespace ServiceLayer
{
    public class HomeDeviceService
    {
#if DEBUG
        public event EventHandler<HomeDeviceDTO> StatusChanged;

        public HomeDeviceService()
        {
            SmartHome.Communications.CommunicationManager.Instance.FindModule<StatusModule>().StateRefreshed += (s, hd) =>
                {
                    if (this.StatusChanged != null)
                    {
                        this.StatusChanged(this, Mapper.Map<HomeDeviceDTO>(hd));
                    }
                };
        }
#endif

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

                Services.HomeDeviceService.UnlinkHomeDevice(idHomeDevice);
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

        public void LinkHomeDevice(int idConnector, int idHomeDevice)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Connector connector = repository.ConnectorRepository.GetById(idConnector);
            HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

            //TODO: falta chequear que el homedevice sea compatible con el conector

            if (connector == null)
                throw new ArgumentException("Connector Id doesn't exist");

            if (homeDevice == null)
                throw new ArgumentException("HomeDevice Id doesn't exist");

            if (connector.InUse)
                throw new ArgumentException("The connector has been occuped by other home device");

            if (homeDevice.InUse)
                throw new ArgumentException("The HomeDevice has been occuped by other connector");

            //UPDATE CHECKSUM
            connector.Node.UpdateChecksum(null);

            connector.LinkHomeDevice(homeDevice);

            repository.Commit();
        }

        public void UnlinkHomeDevice(int idHomeDevice)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

            if (homeDevice == null)
                throw new ArgumentException("HomeDevice Id doesn't exist");

            homeDevice.Connector.UnlinkHomeDevice();

            //UPDATE CHECKSUM
            homeDevice.Connector.Node.UpdateChecksum(null);

            repository.Commit();
        }

        public void UnlinkFromConnector(int idConnector)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Connector connector = repository.ConnectorRepository.GetById(idConnector);

            if (connector == null)
                throw new ArgumentException("Connector Id doesn't exist");

            //UPDATE CHECKSUM
            connector.Node.UpdateChecksum(null);

            connector.UnlinkHomeDevice();

            repository.Commit();
        }

        //PRODUCT REGION
        public string[] GetNameProducts()
        {
            return BusinessProduct.GetProducts.Select(p => p.Name)
                .ToArray();
        }

        public void LinkProduct(int idConnector, string productName)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Connector connector = repository.ConnectorRepository.GetById(idConnector);

            if(connector == null)
                throw new ArgumentException("The connector isn't exist");

            if(connector.InUse)
                throw  new Exception("The connector is being used by other HomeDevice or product");

            Type product = BusinessProduct.GetProductType(productName);

            if (!connector.IsCapable(product))
                throw new Exception("The product is not capable with this connector");

            connector.LinkHomeDevice(product);

            repository.Commit();
        }

        public IEnumerable<ConnectorDTO> GetProductsConnected()
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            var connectors = repository.NodeRespository.GetAll().SelectMany(n => n.Connectors).Where(c => c.Product != null);

            return Mapper.Map<IEnumerable<ConnectorDTO>>(connectors);
        }

        public IEnumerable<ConnectorDTO> GetConnectorsCapableProduct(int idNode, string productName)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Node node = repository.NodeRespository.GetById(idNode);

            if (node == null)
                throw new ArgumentException("Node doesn't exist");

            Type product = BusinessProduct.GetProductType(productName);

            var connectors = node.Connectors.Where(c => !c.InUse && c.IsCapable(product));

            return Mapper.Map<IEnumerable<ConnectorDTO>>(connectors);
        }
    }
}
