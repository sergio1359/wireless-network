#region Using Statements
using System;
using System.Linq;
using System.Linq.Expressions; 
#endregion

namespace DataLayer
{
    public interface IRepository<T> where T : class
    {
        T Insert(T entity);
        void Delete(T entity);
        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        T GetById(int id);
    }
}
