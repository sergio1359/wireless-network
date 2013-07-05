using SmartHome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public class ConnectorRepository : Repository<Connector>
    {
        
        public IQueryable<Connector> GetAllWithHomeDevice()
        {
            return _Collection.Include("HomeDevice");
        }
    }
}
