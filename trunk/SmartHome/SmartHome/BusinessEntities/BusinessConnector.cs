using DataLayer.Entities;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.BusinessEntities
{
    public class BusinessConnector
    {
        /// <summary>
        /// Conecta, sin realizar ninguna comprobacion de compatibilidad, un conector con el HomeDevice pasado por parametros
        /// </summary>
        /// <param name="homeDevice"></param>
        public static void LinkHomeDevice(this Connector connector, HomeDevice homeDevice)
        {
            connector.MappingHomeDevice.Add(homeDevice, connector.GetPinPort());
        }

        /// <summary>
        /// Conecta, sin realizar ninguna comprobacion de compatibilidad, el conector con el producto cuyo nombre es pasado por parametros
        /// </summary>
        /// <param name="nameProduct"></param>
        public static void LinkHomeDevice(this Connector connector, string nameProduct)
        {

        }

        public static void UnlinkHomeDevice(this Connector connector)
        {
            if (connector.ConnectorType == ConnectorTypes.ConectorSensorBoard)
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
