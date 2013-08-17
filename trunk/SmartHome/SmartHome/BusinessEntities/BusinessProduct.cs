#region Using Statements
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.BusinessEntities.BusinessHomeDevice;
#endregion

namespace SmartHome.BusinessEntities
{
    public abstract class BusinessProduct
    {
        private static Type[] products;
        public static Type[] GetProducts
        {
            get
            {
                if (products == null)
                    products = typeof(BusinessProduct).Assembly.GetTypes().Where(t => t != typeof(BusinessProduct) && typeof(BusinessProduct).IsAssignableFrom(t)).ToArray();
                return products;
            }
        }

        protected List<Tuple<Type, List<int>>> mapProduct;
        protected ConnectorTypes connectorProduct;

        public BusinessProduct()
        {
            mapProduct = GetProduct();
            connectorProduct = GetConnectorType();
        }

        public static BusinessProduct GetProduct(Type productType)
        {
            return (BusinessProduct)Activator.CreateInstance(productType);
        }

        public List<HomeDevice> GetInstanceProducts()
        {
            List<HomeDevice> homeDeviceResult = new List<HomeDevice>();

            for (int i = 0; i < mapProduct.Count; i++)
            {
                HomeDevice homeDev = BusinessHomeDevice.BusinessHomeDevice.CreateHomeDevice(mapProduct[i].Item1.Name);
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


    public class SensorBoard : BusinessProduct
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

    public class TemperatureHumidity : BusinessProduct
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
