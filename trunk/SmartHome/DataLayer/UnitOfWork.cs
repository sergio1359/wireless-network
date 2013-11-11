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

        private Lazy<NodeRepository> nodeRepository;
        private Lazy<HomeRepository> homeRepository;
        private Lazy<ThemesRepository> themesRespository;
        private Lazy<TimeOperationRepository> timeOperationRepository;
        private Lazy<ZoneRepository> zoneRepository;
        private Lazy<HomeDeviceRepository> homeDeviceRepository;
        private Lazy<ViewRepository> viewRepository;
        private Lazy<LogRepository> logRepository;
        private Lazy<ConnectorRepository> connectorRepository;
        private Lazy<OperationRepository> operationRepository;
        private Lazy<LocationRepository> locationRepository;
        private Lazy<ConditionalRestrictionRepository> conditionalRestrictionRepository;
        private Lazy<TimeRestrictionRepository> timeRestrictionRepository;
        #endregion

        #region Properties
        public NodeRepository NodeRespository
        {
            get
            {
                return nodeRepository.Value;
            }
        }

        public LocationRepository LocationRepository
        {
            get
            {
                return locationRepository.Value;
            }
        }

        public HomeDeviceRepository HomeDeviceRespository
        {
            get
            {
                return homeDeviceRepository.Value;
            }
        }

        public HomeRepository HomeRespository
        {
            get
            {
                return homeRepository.Value;
            }
        }

        public ThemesRepository ThemesRespository
        {
            get
            {
                return themesRespository.Value;
            }
        }

        public TimeOperationRepository TimeOperationRepository
        {
            get
            {
                return timeOperationRepository.Value;
            }
        }

        public ZoneRepository ZoneRepository
        {
            get
            {
                return zoneRepository.Value;
            }
        }

        public ViewRepository ViewRepository
        {
            get
            {
                return viewRepository.Value;
            }
        }

        public LogRepository LogRepository
        {
            get
            {
                return logRepository.Value;
            }
        }

        public ConnectorRepository ConnectorRepository
        {
            get
            {
                return connectorRepository.Value;
            }
        }

        public OperationRepository OperationRepository
        {
            get
            {
                return operationRepository.Value;
            }
        }

        public TimeRestrictionRepository TimeRestrictionRepository
        {
            get
            {
                return timeRestrictionRepository.Value;
            }
        }

        public ConditionalRestrictionRepository ConditionalRestrictionRepository
        {
            get
            {
                return conditionalRestrictionRepository.Value;
            }
        }
        #endregion

        public static UnitOfWork GetInstance()
        {
            return new UnitOfWork();
        }

        public UnitOfWork()
        {
            this.context = new SmartHomeDBContext();

            nodeRepository = new Lazy<NodeRepository>(() => new NodeRepository(this.context));
            homeRepository = new Lazy<HomeRepository>(() => new HomeRepository(this.context));
            themesRespository = new Lazy<ThemesRepository>(() => new ThemesRepository(this.context));
            timeOperationRepository = new Lazy<TimeOperationRepository>(() => new TimeOperationRepository(this.context));
            zoneRepository = new Lazy<ZoneRepository>(() => new ZoneRepository(this.context));
            homeDeviceRepository = new Lazy<HomeDeviceRepository>(() => new HomeDeviceRepository(this.context));
            viewRepository = new Lazy<ViewRepository>(() => new ViewRepository(this.context));
            logRepository = new Lazy<LogRepository>(() => new LogRepository(this.context));
            connectorRepository = new Lazy<ConnectorRepository>(() => new ConnectorRepository(this.context));
            operationRepository = new Lazy<OperationRepository>(() => new OperationRepository(this.context));
            locationRepository = new Lazy<LocationRepository>(() => new LocationRepository(this.context));
            conditionalRestrictionRepository = new Lazy<ConditionalRestrictionRepository>(() => new ConditionalRestrictionRepository(this.context));
            timeRestrictionRepository = new Lazy<TimeRestrictionRepository>(() => new TimeRestrictionRepository(this.context));
        }

        internal UnitOfWork(SmartHomeDBContext context)
        {
            this.context = context;
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
