#region Using Statements
using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace SmartHome.BusinessEntities
{
    public static class BusinessConnector
    {
        public static Connector CreateConnector(string name, ConnectorTypes connectorType)
        {
            Connector connector = new Connector
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
            connector.HomeDevices = new List<HomeDevice> { homeDevice };
            connector.Product = null;
        }

        /// <summary>
        /// Connect without checks the connector with the list of homeDevice
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="typeProduct"></param>
        public static void LinkHomeDevice(this Connector connector, Type typeProduct)
        {
            BusinessProduct product = BusinessProduct.GetProduct(typeProduct);
            connector.HomeDevices = product.GetInstanceProducts();

            connector.Product = new Product
            {
                TypeProduct = typeProduct.Name,
                NameProduct = "",
            };
        }

        /// <summary>
        /// Unlink without check the HomeDevice or the Product connected in the connector
        /// </summary>
        /// <param name="connector"></param>
        public static void UnlinkHomeDevice(this Connector connector)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            if (connector.Product != null) //is Product
            {
                foreach (var item in connector.HomeDevices)
                {
                    repository.HomeDeviceRespository.Delete(item);
                }
            }

            connector.HomeDevices = new List<HomeDevice>();

            connector.Product = null;

            repository.Commit();
        }

        public static bool IsCapable(this Connector connector, HomeDevice homeDevice)
        {
            if (connector.ConnectorType == ConnectorTypes.DimmerPassZero)
                return false;

            return connector.ConnectorType == homeDevice.ConnectorCapable;
        }

        public static bool IsCapable(this Connector connector, Type productName)
        {
            if (connector.ConnectorType == ConnectorTypes.DimmerPassZero)
                return false;

            BusinessProduct product = (BusinessProduct)Activator.CreateInstance(productName);

            return connector.ConnectorType == product.GetConnectorType();
        }

        public static PinPort[] GetPinPort(this Connector connector)
        {
            return ProductConfiguration.GetShieldDictionary(connector.Node.Shield)[connector.Name].Item2;
        }

        public static PinPortConfiguration GetPinPortConfiguration(this Connector connector, HomeDevice homeDevice)
        {
            return ProductConfiguration.GetPinPortConfiguration(homeDevice);
        }

        public static IEnumerable<Operation> GetActionsConnector(this Connector connector)
        {
            if (!connector.InUse)
                return Enumerable.Empty<Operation>();

            return connector.HomeDevices.SelectMany(hd => hd.Operations)
                    .ToList();
        }
    }
}
