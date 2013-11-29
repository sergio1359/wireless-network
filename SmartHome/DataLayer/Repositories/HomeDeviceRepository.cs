#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer
{
    public class HomeDeviceRepository: Repository<HomeDevice>
    {
        public HomeDeviceRepository(SmartHomeDBContext context) : base(context) { }

        public IQueryable<HomeDevice> GetHomeDevicesWithLocations()
        {
            return _Collection.Include("Location");
        }

        public IQueryable<HomeDevice> GetConnectedHomeDevices()
        {
            return _Collection.Include("Connector.Node").Where(c => c.Connector != null);
        }

        public override void Delete(HomeDevice entityHomeDevice)
        {
            UnitOfWork repository = new UnitOfWork(this._context);
            for (int i = entityHomeDevice.Operations.Count - 1; i >= 0; i--)
            {
                repository.OperationRepository.Delete(entityHomeDevice.Operations.ElementAt(i));
            }

            for (int i = entityHomeDevice.Locations.Count - 1; i >= 0; i--)
            {
                repository.LocationRepository.Delete(entityHomeDevice.Locations.ElementAt(i));
            }            

            base.Delete(entityHomeDevice);
        }
    }
}
