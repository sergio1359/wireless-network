#region Using Statements
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace DataLayer
{
    public class TimeOperationRepository : Repository<TimeOperation>
    {
        public TimeOperationRepository(SmartHomeDBContext context) : base(context) { }

        public IQueryable<TimeOperation> GetOperationInHomeDevice(int idHomeDevice)
        {
            return _Collection.Where(to => to.Operation.DestionationHomeDevice.Id == idHomeDevice);
        }

    }
}
