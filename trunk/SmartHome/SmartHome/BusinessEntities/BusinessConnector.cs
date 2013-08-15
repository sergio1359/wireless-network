#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using SmartHome.Products;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace SmartHome.BusinessEntities
{
    public static class BusinessConnector
    {

        public static Connector CreateConnector(string name, ConnectorTypes connectorType)
        {
            Connector connector = new Connector()
            {
                ConnectorType = connectorType,
                Name = name
            };

            return connector;
        }
        
        /// <summary>
        /// Connect without checks the connector with the homeDevice
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="homeDevice"></param>
        public static void LinkHomeDevice(this Connector connector, HomeDevice homeDevice)
        {
            connector.HomeDevices = new List<HomeDevice>() { homeDevice };
        }

        /// <summary>
        /// Connect without checks the connector with the list of homeDevice
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="homeDevices"></param>
        public static void LinkHomeDevice(this Connector connector, List<HomeDevice> homeDevices)
        {
            connector.HomeDevices = homeDevices;
        }

        public static void UnlinkHomeDevice(this Connector connector)
        {
            if (connector.ConnectorType == ConnectorTypes.ConnectorSensorBoard)
            {
                //TODO: hay que destruir los elementos uno a uno. 

            }

            connector.MappingHomeDevice.Clear();
        }

        public static bool IsCapable(this Connector connector, HomeDevice homeDevice)
        {
            if (connector.ConnectorType == ConnectorTypes.DimmerPassZero)
                return false;

            return connector.ConnectorType == homeDevice.ConnectorCapable;
        }

        public static List<PinPort> GetPinPort(this Connector connector)
        {
            return ProductConfiguration.GetShieldDictionary(connector.Node.Shield)[connector.Name].Item2;
        }

        public static PinPortConfiguration GetPinPortConfiguration(this Connector connector, HomeDevice homeDevice)
        {
            return ProductConfiguration.GetPinPortConfiguration(homeDevice);
        }

        public static Operation[] GetActionsConnector(this Connector connector)
        {
            if (!connector.InUse)
            {
                return new Operation[0];
            }
            else
            {
                return connector.HomeDevices.SelectMany(hd => hd.Operations).ToArray();
            }

        }
    }
}
