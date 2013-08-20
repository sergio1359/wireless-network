#region Using Statements
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.HomeDevices; 
#endregion

namespace DataLayer
{
    public static class Repositories
    {
        private static SmartHomeDBContext context;
        private static NodeRepository nodeRepository;
        private static HomeRepository homeRepository;
        private static ThemesRepository themesRespository;
        private static TimeOperationRepository timeOperationRepository;
        private static ZoneRepository zoneRepository;
        private static HomeDeviceRepository homeDeviceRepository;
        private static ViewRepository viewRepository;
        private static LogRepository logRepository;
        private static ConnectorRepository connectorRepository;
        private static OperationRepository operationRepository;

        public static NodeRepository NodeRespository
        {
            get
            {
                if (nodeRepository == null)
                    nodeRepository = new NodeRepository(context);
                return nodeRepository;
            }
        }

        
        public static HomeDeviceRepository HomeDeviceRespository
        {
            get
            {
                if (homeDeviceRepository == null)
                    homeDeviceRepository = new HomeDeviceRepository(context);
                return homeDeviceRepository;
            }
        }

        public static HomeRepository HomeRespository
        {
            get
            {
                if (homeRepository == null)
                    homeRepository = new HomeRepository(context);
                return homeRepository;
            }
        }

        public static ThemesRepository ThemesRespository
        {
            get
            {
                if (themesRespository == null)
                    themesRespository = new ThemesRepository(context);
                return themesRespository;
            }
        }

        public static TimeOperationRepository TimeOperationRepository
        {
            get
            {
                if (timeOperationRepository == null)
                    timeOperationRepository = new TimeOperationRepository(context);
                return timeOperationRepository;
            }
        }

        public static ZoneRepository ZoneRepository
        {
            get
            {
                if (zoneRepository == null)
                    zoneRepository = new ZoneRepository(context);
                return zoneRepository;
            }
        }

        public static ViewRepository ViewRepository
        {
            get
            {
                if (viewRepository == null)
                    viewRepository = new ViewRepository(context);
                return viewRepository;
            }
        }

        public static LogRepository LogRepository
        {
            get
            {
                if (logRepository == null)
                    logRepository = new LogRepository(context);
                return logRepository;
            }
        }

        public static ConnectorRepository ConnectorRepository
        {
            get
            {
                if (connectorRepository == null)
                    connectorRepository = new ConnectorRepository(context);
                return connectorRepository;
            }
        }

        public static OperationRepository OperationRepository
        {
            get
            {
                if (operationRepository == null)
                    operationRepository = new OperationRepository(context);
                return operationRepository;
            }
        }

        public static void SaveChanges()
        {
            context.SaveChanges();
        }

        static Repositories()
        {
            context = new SmartHomeDBContext();
        }
    }
}
