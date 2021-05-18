using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UlasBlog.Data.Abstract;

namespace UlasBlog.Data.Concrete.EntityFramework
{
    public class EfGenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext context;
        public EfGenericRepository(AppDbContext _context)
        {
            context = _context;
        }

        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void AddRange(List<T> entity)
        {
            context.Set<T>().AddRange(entity);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public void Edit(T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().Where(predicate);
        }

        public T Get(int id)
        {
            return context.Set<T>().Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return context.Set<T>();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
