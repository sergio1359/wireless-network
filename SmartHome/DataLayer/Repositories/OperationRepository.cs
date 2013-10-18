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
    public class OperationRepository : Repository<Operation>
    {
        public OperationRepository(SmartHomeDBContext context) : base(context) { }

        public override void Delete(Operation entityOperation)
        {
            UnitOfWork repository = new UnitOfWork(this._context);
            for (int i = entityOperation.ConditionalRestriction.Count - 1; i <= 0; i--)
            {
                repository.ConditionalRestrictionRepository.Delete(entityOperation.ConditionalRestriction.ElementAt(i));
            }

            for (int i = entityOperation.TimeRestrictions.Count - 1; i <= 0; i--)
            {
                repository.TimeRestrictionRepository.Delete(entityOperation.TimeRestrictions.ElementAt(i));
            }

            base.Delete(entityOperation);
        }

    }
}
