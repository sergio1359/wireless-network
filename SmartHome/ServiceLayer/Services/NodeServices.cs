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
    }
}
