#region Using Statements
using System;
using System.Data.Entity;
using System.Linq; 
#endregion

namespace DataLayer
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> _Collection { get; set; }
        protected SmartHomeDBContext _context;

        public Repository(SmartHomeDBContext context)
        {
            _context = context;
            _Collection = context.Set<T>();
        }

        public T Insert(T entity)
        {
           return _Collection.Add(entity);
        }

        public void Delete(T entity)
        {
            _Collection.Remove(entity);
        }

        public int Count()
        {
            return _Collection.Count();
        }

        public IQueryable<T> GetAll()
        {
            return _Collection;
        }

        public T GetById(int id)
        {
            return _Collection.Find(id);
        }

        public IQueryable<T> SearchFor(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
