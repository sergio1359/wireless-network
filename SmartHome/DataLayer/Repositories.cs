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
