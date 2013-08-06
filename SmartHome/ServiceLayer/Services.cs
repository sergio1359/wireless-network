using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class Services
    {
        public HomeService Home;
        public HomeDeviceService HomeDevices;
        public LogService Logs;
        public NodeService Nodes;
        public OperationService Operation;

        public Services()
        {
            Home = new HomeService();
            HomeDevices = new HomeDeviceService();
            Logs = new LogService();
            Nodes = new NodeService();
            Operation = new OperationService();
        }
    }
}
