#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer
{
    public class UnitOfWork : IDisposable
    {
        #region Private Fields
        private SmartHomeDBContext context;

        private NodeRepository nodeRepository;
        private HomeRepository homeRepository;
        private ThemesRepository themesRespository;
        private TimeOperationRepository timeOperationRepository;
        private ZoneRepository zoneRepository;
        private HomeDeviceRepository homeDeviceRepository;
        private ViewRepository viewRepository;
        private LogRepository logRepository;
        private ConnectorRepository connectorRepository;
        private OperationRepository operationRepository;
        private LocationRepository locationRepository; 
        #endregion

        #region Properties
        public NodeRepository NodeRespository
        {
            get
            {
                if (nodeRepository == null)
                    nodeRepository = new NodeRepository(context);

                return nodeRepository;
            }
        }

        public LocationRepository LocationRepository
        {
            get
            {
                if (locationRepository == null)
                    locationRepository = new LocationRepository(context);
                return locationRepository;
            }
        }

        public HomeDeviceRepository HomeDeviceRespository
        {
            get
            {
                if (homeDeviceRepository == null)
                    homeDeviceRepository = new HomeDeviceRepository(context);
                return homeDeviceRepository;
            }
        }

        public HomeRepository HomeRespository
        {
            get
            {
                if (homeRepository == null)
                    homeRepository = new HomeRepository(context);
                return homeRepository;
            }
        }

        public ThemesRepository ThemesRespository
        {
            get
            {
                if (themesRespository == null)
                    themesRespository = new ThemesRepository(context);
                return themesRespository;
            }
        }

        public TimeOperationRepository TimeOperationRepository
        {
            get
            {
                if (timeOperationRepository == null)
                    timeOperationRepository = new TimeOperationRepository(context);
                return timeOperationRepository;
            }
        }

        public ZoneRepository ZoneRepository
        {
            get
            {
                if (zoneRepository == null)
                    zoneRepository = new ZoneRepository(context);
                return zoneRepository;
            }
        }

        public ViewRepository ViewRepository
        {
            get
            {
                if (viewRepository == null)
                    viewRepository = new ViewRepository(context);
                return viewRepository;
            }
        }

        public LogRepository LogRepository
        {
            get
            {
                if (logRepository == null)
                    logRepository = new LogRepository(context);
                return logRepository;
            }
        }

        public ConnectorRepository ConnectorRepository
        {
            get
            {
                if (connectorRepository == null)
                    connectorRepository = new ConnectorRepository(context);
                return connectorRepository;
            }
        }

        public OperationRepository OperationRepository
        {
            get
            {
                if (operationRepository == null)
                    operationRepository = new OperationRepository(context);
                return operationRepository;
            }
        } 
        #endregion

        public UnitOfWork()
        {
            this.context = new SmartHomeDBContext();
        }

        #region Public Methods
        public void Commit()
        {
            this.context.SaveChanges();
        }

        public void Dispose()
        {
            this.context.Dispose();
        } 
        #endregion
    }
}
