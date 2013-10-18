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
    public class ThemesRepository : Repository<Theme>
    {
        public ThemesRepository(SmartHomeDBContext context) : base(context) { }

        public override void Delete(Theme entityTheme)
        {
            UnitOfWork repository = new UnitOfWork(this._context);

            for (int i = entityTheme.Operations.Count - 1; i >= 0; i--)
            {
                repository.OperationRepository.Delete(entityTheme.Operations.ElementAt(i));
            }

            base.Delete(entityTheme);
        }
    }
}
