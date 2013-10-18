#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace DataLayer
{
    public class LogRepository : Repository<Log>
    {
        public LogRepository(SmartHomeDBContext context) : base(context) { }

        public IQueryable<Log> GetLogByCategory(LogTypes typeLog)
        {
            return _Collection.Where(l => l.Category == typeLog);
        }

    }
}
