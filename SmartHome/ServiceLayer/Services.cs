#region Using Statements
using AutoMapper;
using ServiceLayer.DTO; 
#endregion

namespace ServiceLayer
{
    public static class Services
    {
        private static HomeService homeService;
        private static NodeService nodeService;
        private static OperationService operationService;
        private static LogService logService;
        private static HomeDeviceService homeDeviceService;

        public static HomeDeviceService HomeDeviceService
        {
            get
            {
                if (homeDeviceService == null)
                    homeDeviceService = new HomeDeviceService();
                return homeDeviceService;
            }
        }

        public static HomeService HomeService
        {
            get
            {
                if (homeService == null)
                    homeService = new HomeService();
                return homeService;
            }
        }

        public static NodeService NodeService
        {
            get
            {
                if (nodeService == null)
                    nodeService = new NodeService();
                return nodeService;
            }
        }

        public static OperationService OperationService
        {
            get
            {
                if (operationService == null)
                    operationService = new OperationService();
                return operationService;
            }
        }

        public static LogService LogService
        {
            get
            {
                if (logService == null)
                    logService = new LogService();
                return logService;
            }
        }

        static Services()
        {
            Mapper.Initialize(t => t.AddProfile<ServiceProfile>());
        }
    }
}
