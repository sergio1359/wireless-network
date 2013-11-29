#region Using Statements
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using System.Reflection;
#endregion

namespace SmartHome.BusinessEntities
{
    public abstract class BusinessProduct
    {
        private static Type[] _products;
        public static Type[] GetProducts
        {
            get
            {
                if (_products == null)
                    _products = typeof(BusinessProduct).Assembly.GetTypes().Where(t => t != typeof(BusinessProduct) && typeof(BusinessProduct).IsAssignableFrom(t)).ToArray();
                return _products;
            }
        }

        protected List<Tuple<Type, int[]>> MapProduct;
        protected ConnectorTypes ConnectorProduct;

        protected BusinessProduct()
        {
            MapProduct = GetProduct();
            ConnectorProduct = GetConnectorType();
        }

        public static BusinessProduct GetProduct(Type productType)
        {
            return (BusinessProduct)Activator.CreateInstance(productType);
        }

        public static Type GetProductType(string productName)
        {
            Type type = Assembly.GetAssembly(typeof (BusinessProduct)).GetTypes().FirstOrDefault(t => t.Name == productName);

            if(type == null)
                throw new ArgumentException("Product name isn't exist");

            return type;
        }

        public List<HomeDevice> GetInstanceProducts()
        {
            List<HomeDevice> homeDeviceResult = new List<HomeDevice>();

            for (int i = 0; i < MapProduct.Count; i++)
            {
                HomeDevice homeDev = BusinessHomeDevice.BusinessHomeDevice.CreateHomeDevice(MapProduct[i].Item1.Name);
                homeDev.ProductTag = i;

                homeDeviceResult.Add(homeDev);
            }

            return homeDeviceResult;
        }

        public int[] GetPinPortMap(int tagProduct)
        {
            return MapProduct[tagProduct].Item2;
        }

        public abstract List<Tuple<Type, int[]>> GetProduct();

        public abstract ConnectorTypes GetConnectorType();
    }


    public class SensorBoard : BusinessProduct
    {
        public override List<Tuple<Type, int[]>> GetProduct()
        {
            List<Tuple<Type, int[]>> result = new List<Tuple<Type, int[]>>
            {
                Tuple.Create(typeof (Button), new int[] {1})
            };
            return result;
        }

        public override ConnectorTypes GetConnectorType()
        {
            return ConnectorTypes.ConnectorSensorBoard;
        }
    }

    public class TemperatureHumidity : BusinessProduct
    {
        public override List<Tuple<Type, int[]>> GetProduct()
        {
            List<Tuple<Type, int[]>> result = new List<Tuple<Type, int[]>>
            {
                Tuple.Create(typeof (TemperatureSensor), new int[] {0}),
                Tuple.Create(typeof (HumiditySensor), new int[] {0})
            };
            return result;
        }

        public override ConnectorTypes GetConnectorType()
        {
            return ConnectorTypes.LogicInput;
        }
    }
}
