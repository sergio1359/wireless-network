#region Using Statements
using AutoMapper;
using ServiceLayer.DTO; 
#endregion

namespace ServiceLayer
{
    public static class Services
    {
        #region private fields

        private static HomeService homeService;
        private static NodeServices nodeService;
        private static OperationServices operationService;
        private static LogServices logService;
        private static HomeDeviceService homeDeviceService;
        private static ZoneServices zoneService;
        private static ViewServices viewService;
        private static SchedulerServices schedulerService;
        private static ThemeServices themeService;

        #endregion

        #region public property

        public static SchedulerServices SchedulerService
        {
            get
            {
                if (schedulerService == null)
                    schedulerService = new SchedulerServices();
                return schedulerService;
            }
        }

        public static ThemeServices ThemeService
        {
            get
            {
                if (themeService == null)
                    themeService = new ThemeServices();
                return themeService;
            }
        }

        public static ViewServices ViewService
        {
            get
            {
                if (viewService == null)
                    viewService = new ViewServices();
                return viewService;
            }
        }

        public static ZoneServices ZoneService
        {
            get
            {
                if (zoneService == null)
                    zoneService = new ZoneServices();
                return zoneService;
            }
        }

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

        public static NodeServices NodeService
        {
            get
            {
                if (nodeService == null)
                    nodeService = new NodeServices();
                return nodeService;
            }
        }

        public static OperationServices OperationService
        {
            get
            {
                if (operationService == null)
                    operationService = new OperationServices();
                return operationService;
            }
        }

        public static LogServices LogService
        {
            get
            {
                if (logService == null)
                    logService = new LogServices();
                return logService;
            }
        }

        #endregion

        static Services()
        {
            Mapper.Initialize(t => t.AddProfile<ServiceProfile>());

            SmartHome.Communications.CommunicationManager.Instance.Initialize();
        }
    }
}
