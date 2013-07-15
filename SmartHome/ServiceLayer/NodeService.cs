using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class NodeService
    {
        /// <summary>
        /// Conecta un conector con un home device
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="homeDevice"></param>
        /// <returns>0 == OK, 
        /// 1 == el conector estaba ya ocupado por un home Device
        /// 2 == el homeDevice ya estaba conectado en un conector diferente al nuevo</returns>
        public int LinkHomeDevice(Connector connector, HomeDevice homeDevice)
        {
            if (connector.HomeDevice != null)
                return 1;

            if (homeDevice.InUse)
                return 2;


            connector.HomeDevice = homeDevice;
            homeDevice.Connector = connector;
            return 0;
        }

        /// <summary>
        /// Desrelaciona un HomeDevice de su conector asociado
        /// </summary>
        public void Unlink(HomeDevice homeDevice)
        {
            homeDevice.Connector.HomeDevice = null;
            homeDevice.Connector = null;
        }

        public Connector[] GetConnectors(Node node)
        {
            return node.Connectors.ToArray();
        }

        public string[] GetTypeShields()
        {
            return Enum.GetNames(typeof(ShieldType));
        }

        public string[] GetTypeBases()
        {
            return Enum.GetNames(typeof(BaseType));
        }


        public string GetNameNode(Node node)
        {
            return node.Name;
        }

        public ushort GetAddressNode(Node node)
        {
            return node.Address;
        }

        public void UpdatePosition(Node node, string zone, int X, int Y)
        {
            node.Position.Zone = zone;
            node.Position.X = X;
            node.Position.Y = Y;
        }

        public Connector[] GetFreeConnectors(Node node)
        {
            return node.Connectors.Where(n => !n.InUse).ToArray();
        }

        public Node[] GetNodes()
        {
            return NetworkManager.Nodes.ToArray();
        }

        public Node[] GetNode(string zone)
        {
            return NetworkManager.Nodes.Where(n => n.Position.Zone == zone).ToArray();
        }
    }
}
