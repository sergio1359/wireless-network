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
           T addEntity = _Collection.Add(entity);
           _context.SaveChanges();

           return addEntity;
        }

        public void Delete(T entity)
        {
            _Collection.Remove(entity);
            _context.SaveChanges();
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
