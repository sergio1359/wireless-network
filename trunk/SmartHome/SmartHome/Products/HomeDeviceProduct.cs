#region Using Statements
using DataLayer.Entities.Enums;
using SmartHome.BusinessEntities;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace DataLayer.Entities.HomeDevices
{
    public abstract class Product
    {
        private static Type[] products;
        public static Type[] GetProducts
        {
            get
            {
                if (products == null)
                    products = typeof(Product).Assembly.GetTypes().Where(t => t != typeof(Product) && typeof(Product).IsAssignableFrom(t)).ToArray();
                return products;
            }
        }

        protected List<Tuple<Type, List<int>>> mapProduct;
        protected ConnectorTypes connectorProduct;

        public Product()
        {
            mapProduct = GetProduct();
            connectorProduct = GetConnectorType();
        }

        public static Product GetProduct(Type productType)
        {
            return (Product)Activator.CreateInstance(productType);
        }

        public List<HomeDevice> GetInstanceProducts()
        {
            List<HomeDevice> homeDeviceResult = new List<HomeDevice>();

            for (int i = 0; i < mapProduct.Count; i++)
            {
                HomeDevice homeDev = BusinessHomeDevice.CreateHomeDevice(mapProduct[i].Item1.Name);
                homeDev.ProductTag = i;

                homeDeviceResult.Add(homeDev);
            }

            return homeDeviceResult;
        }

        public List<int> GetPinPortMap(int tagProduct)
        {
            return mapProduct[tagProduct].Item2;
        }

        protected abstract List<Tuple<Type, List<int>>> GetProduct();

        protected abstract ConnectorTypes GetConnectorType();
    }


    public class SensorBoard : Product
    {
        protected override List<Tuple<Type, List<int>>> GetProduct()
        {
            List<Tuple<Type, List<int>>> result = new List<Tuple<Type, List<int>>>();
            result.Add(Tuple.Create(typeof(Button), new List<int>() { 1 }));
            return result;
        }

        protected override ConnectorTypes GetConnectorType()
        {
            return ConnectorTypes.ConnectorSensorBoard;
        }
    }

    public class TemperatureHumidity : Product
    {
        protected override List<Tuple<Type, List<int>>> GetProduct()
        {
            List<Tuple<Type, List<int>>> result = new List<Tuple<Type, List<int>>>();
            result.Add(Tuple.Create(typeof(TemperatureSensor), new List<int>() { 0 }));
            result.Add(Tuple.Create(typeof(HumiditySensor), new List<int>() { 0 }));
            return result;
        }

        protected override ConnectorTypes GetConnectorType()
        {
            return ConnectorTypes.LogicInput;
        }
    }
}
