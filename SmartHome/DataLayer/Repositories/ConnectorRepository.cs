#region Using Statements
using DataLayer.Entities;
using System.Linq;
#endregion

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
