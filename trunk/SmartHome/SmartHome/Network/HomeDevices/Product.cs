using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Network.HomeDevices
{
    public class Product
    {
        public ConnectorType TypeConnector { get; set; }
        public List<Tuple<Type, List<int>>> resultProduct;

        public Product()
        {
            resultProduct = new List<Tuple<Type, List<int>>>();
        }

        public static string[] GetProducts
        {
            get
            {
                return typeof(Product).Assembly.GetTypes().Where(t => t != typeof(Product).GetType() && typeof(Product).GetType().IsAssignableFrom(t)).Select(t => t.Name).ToArray();
            }
        }

        public static Dictionary<HomeDevice, List<PinPort>> GetMapping(Connector connector, Product product)
        {
            List<PinPort> PinPorts = connector.GetPinPort();
            Dictionary<HomeDevice, List<PinPort>> map = new Dictionary<HomeDevice,List<PinPort>>();

            foreach (var tuple in product.resultProduct)
            {
                HomeDevice hd = (HomeDevice)tuple.Item1.GetConstructors()[0].Invoke(new object[0]);

                List<PinPort> pines = new List<PinPort>();
                tuple.Item2.ForEach(p => pines.Add(PinPorts[p])); //Deep Copy?

                map.Add(hd, pines);
            }
            return map;
        }

    }


    public class SensorBoard : Product
    {
        public SensorBoard() : base()
        {
            base.resultProduct.Add(Tuple.Create(typeof(Button), new List<int>() { 1 }));

            throw new NotImplementedException();
        }
    }

    public class TemperatureHumidity : Product
    {
        public TemperatureHumidity(): base()
        {
            base.resultProduct.Add(Tuple.Create(typeof(TemperatureSensor), new List<int>() { 0 }));
            base.resultProduct.Add(Tuple.Create(typeof(HumiditySensor), new List<int>() { 0 }));
            base.TypeConnector = ConnectorType.LogicInput;
        }
    }
}
