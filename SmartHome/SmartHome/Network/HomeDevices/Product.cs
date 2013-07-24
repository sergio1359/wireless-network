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

        public string[] GetProducts
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static Dictionary<HomeDevice, List<PinPort>> GetMapping(Connector connector, Product product)
        {
            List<PinPort> PinPorts = connector.GetPinPort();
            Dictionary<HomeDevice, List<PinPort>> result = new Dictionary<HomeDevice,List<PinPort>>();

            foreach (var item in product.)
            {
                
            }



        }

    }


    public class SensorBoard : Product
    {
        List<Tuple<Type, List<int>>> resultProduct;

        public SensorBoard()
        {
            resultProduct = new List<Tuple<Type, List<int>>>();
            resultProduct.Add(Tuple.Create(typeof(Button), new List<int>() { 1 }));

            throw new NotImplementedException();
        }



    }
}
