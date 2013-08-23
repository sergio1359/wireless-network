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
    }
}
