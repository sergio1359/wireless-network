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
    public class ZoneRepository : Repository<Zone>
    {
        public ZoneRepository(SmartHomeDBContext context) : base(context) { }

        public override void Delete(Zone entityZone)
        {
            UnitOfWork repository = new UnitOfWork(this._context);

            for (int i = entityZone.Views.Count - 1; i >= 0; i--)
            {
                repository.ViewRepository.Delete(entityZone.Views.ElementAt(i));
            }

            base.Delete(entityZone);
        }

    }
}
