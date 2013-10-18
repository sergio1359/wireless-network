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
	public class ViewRepository : Repository<View>
	{
		public ViewRepository(SmartHomeDBContext context) : base(context) { }

		public override void Delete(View entityView)
		{
			UnitOfWork repository = new UnitOfWork(this._context);

			for (int i = entityView.Locations.Count - 1; i >= 0; i--)
			{
				repository.LocationRepository.Delete(entityView.Locations.ElementAt(i));
			}

			base.Delete(entityView);
		}

	}
}
