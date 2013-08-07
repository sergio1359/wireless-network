using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public static class Services
    {
        private static NodeService nodeService;

        public static NodeService NodeService
        {
            get
            {
                if (nodeService == null)
                    nodeService = new NodeService();
                return nodeService;
            }
        }

        //public HomeService Home;
        //public HomeDeviceService HomeDevices;
        //public LogService Logs;
        //public NodeService Nodes;
        //public OperationService Operation;

        //public Services()
        //{
        //    Home = new HomeService();
        //    HomeDevices = new HomeDeviceService();
        //    Logs = new LogService();
        //    Nodes = new NodeService();
        //    Operation = new OperationService();
        //}
    }
}
