using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> _Connectors { get; set; }
        protected DbContext _context;

        public Repository() { } //Lo he tenido que poner porque daba fallo al compilar, compi para los coleguis

        public Repository(DbContext context)
        {
            _context = context;
            _Connectors = context.Set<T>();
        }

        public void Insert(T entity)
        {
            _Connectors.Add(entity);
        }

        public void Delete(T entity)
        {
            _Connectors.Remove(entity);
        }

        public IQueryable<T> GetAll()
        {
            return _Connectors;
        }

        public T GetById(int id)
        {
            return _Connectors.Find(id);
        }


        public IQueryable<T> SearchFor(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
