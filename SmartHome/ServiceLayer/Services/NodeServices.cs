#region Using Statements
using DataLayer.Entities.Enums;
using ServiceLayer.DTO;
using System;
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using DataLayer;
using SmartHome.BusinessEntities;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using AutoMapper;
using System.Linq;
using SmartHome.Communications;
using SmartHome.Communications.Modules.Config;
using SmartHome.Communications.Modules;
using System.Threading.Tasks;
using SmartHome.Communications.Modules.Network;
using System.Collections.Generic;
#endregion

namespace ServiceLayer
{
    public class NodeServices
    {

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

            //UPDATE CHECKSUM
            homeDevice.Connector.Node.UpdateChecksum(null);

            homeDevice.Connector.UnlinkHomeDevice();
            repository.Commit();
        }

        public IEnumerable<PendingNodeInfoDTO> GetPendingNodes()
        {
            var pendingInfo = CommunicationManager.Instance.FindModule<NetworkJoin>().PendingNodes;
            return Mapper.Map<IEnumerable<PendingNodeInfoDTO>>(pendingInfo);
        }

        public async Task<bool> AllowPendingNode(string MAC)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Home home = repository.HomeRespository.GetHome();

            Node node = repository.NodeRespository.GetByMacAddress(MAC);

            NetworkJoin joinMod = CommunicationManager.Instance.FindModule<NetworkJoin>();

            PendingNodeInfo info = joinMod.PendingNodes.FirstOrDefault(n => n.MacAddress == MAC);

            if (info != null)
            {
                if (node == null)
                {
                    node = BusinessNode.CreateNode((BaseTypes)info.BaseType, (ShieldTypes)info.ShieldType);
                    node.Mac = info.MacAddress;
                    node.Address = repository.NodeRespository.GetNewAddress();

                    node = repository.NodeRespository.Insert(node);
                    repository.Commit();
                }

                return await joinMod.AcceptNode(MAC, (ushort)node.Address, home.Security);
            }

            return false;
        }

        public IEnumerable<ConnectorDTO> GetConnectors(int idNode)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Node node = repository.NodeRespository.GetById(idNode);

            if (node == null)
                throw new ArgumentException("Node Id doesn't exist");

            return Mapper.Map<IEnumerable<ConnectorDTO>>(node.Connectors);
        }

        public IEnumerable<ConnectorDTO> GetConnectorsCapable(int idHomeDevice, int idNode)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Node node = repository.NodeRespository.GetById(idNode);
            HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

            if (node == null)
                throw new ArgumentException("Node Id doesn't exist");

            if (homeDevice == null)
                throw new ArgumentException("HomeDevice Id doesn't exist");

            var connectorsResult = node.Connectors.Where(c => c.IsCapable(homeDevice) && c.InUse == false);

            return Mapper.Map<IEnumerable<ConnectorDTO>>(connectorsResult);
        }

        public string GetNameNode(int idNode)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            var node = repository.NodeRespository.GetById(idNode);

            if (node == null)
                throw new ArgumentException("Node Id doesn't exist");

            return node.Name;
        }

        public void SetNameNode(int idNode, string newName)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Node node = repository.NodeRespository.GetById(idNode);

            if (node == null)
                throw new ArgumentException("Node Id doesn't exist");

            node.Name = newName;
            repository.Commit();
        }

        public int GetAddressNode(int idNode)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            var node = repository.NodeRespository.GetById(idNode);

            if (node == null)
                throw new ArgumentException("Node Id doesn't exist");

            return node.Address;
        }

        public void SetAddressNode(int idNode, ushort newAddress)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Node node = repository.NodeRespository.GetById(idNode);

            if (node == null)
                throw new ArgumentException("Node Id doesn't exist");

            //UPDATE CHECKSUM
            node.UpdateChecksum(null);

            node.Address = newAddress;
            repository.Commit();
        }

        public LocationDTO GetNodePosition(int idNode)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            var node = repository.NodeRespository.GetById(idNode);

            if (node == null)
                throw new ArgumentException("Node Id doesn't exist");

            return Mapper.Map<LocationDTO>(node.Location);
        }

        public IEnumerable<NodeDTO> GetNodes()
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            var nodes = repository.NodeRespository.GetAll();

            return Mapper.Map<IEnumerable<NodeDTO>>(nodes);
        }

        public IEnumerable<NodeDTO> GetNodes(int idZone)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            if (repository.ZoneRepository.GetById(idZone) == null)
                throw new ArgumentException("Zone Id doesn't exist");

            var nodes = repository.NodeRespository.GetAll().Where(n => n.Location.Id == idZone);

            return Mapper.Map<IEnumerable<NodeDTO>>(nodes);
        }

        public string[] GetTypesShields()
        {
            return Enum.GetNames(typeof(ShieldTypes));
        }

        public string[] GetTypesBases()
        {
            return Enum.GetNames(typeof(BaseTypes));
        }

        public void LinkProduct(int idConnector, string typeProduct)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Connector connector = repository.ConnectorRepository.GetById(idConnector);

            if (connector == null)
                throw new ArgumentException("Connector Id doesn't exist");

            if (connector.InUse)
                throw new ArgumentException("Connector has been in use by other product or HomeDevice");

            Type type = Type.GetType(typeProduct);

            connector.LinkHomeDevice(type);
        }

        public void UnlinkProduct(int idConnector)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Connector connector = repository.ConnectorRepository.GetById(idConnector);

            if (connector == null)
                throw new ArgumentException("Connector Id doesn't exist");

            connector.UnlinkHomeDevice();
        }

        public IEnumerable<ConnectorDTO> GetConnectorProductsConnected()
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            var connectors = repository.NodeRespository.GetAll().SelectMany(n => n.Connectors).Where(c => c.Product != null);

            return Mapper.Map<IEnumerable<ConnectorDTO>>(connectors);
        }

        public IEnumerable<ConnectorDTO> GetConnectorCapableProducts(int idNode, string product)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Node node = repository.NodeRespository.GetById(idNode);
            Type typeProduct;

            if (node == null)
                throw new ArgumentException("Node Id doesn't exist");

            try
            {
                typeProduct = Type.GetType(product);
            }
            catch (Exception)
            {
                throw new ArgumentException("Type product doesn't exist");
            }

            var connectors = node.Connectors.Where(c => !c.InUse && c.IsCapable(typeProduct));

            return Mapper.Map<IEnumerable<ConnectorDTO>>(connectors);
        }
    }
}
