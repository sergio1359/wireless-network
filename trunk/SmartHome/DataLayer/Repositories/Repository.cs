using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    internal abstract class Repository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> _Connectors { get; set; }
        protected DbContext _context;

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
    }
}
