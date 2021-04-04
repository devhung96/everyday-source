using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Project.App.DesignPatterns.Repositories
{
    public class RepositoryMariaBase<T> : IRepositoryRoot<T> where T : class
    {
        private readonly MariaDBContext DbContext;
        public RepositoryMariaBase(MariaDBContext dbContext)
        {
            DbContext = dbContext;
        }

        public IQueryable<T> FindAll()
        {
            return DbContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            //return DbContext.Set<T>().Where(expression).AsNoTracking();
            return DbContext.Set<T>().Where(expression);
        }

        public void Add(T entity)
        {
            DbContext.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            DbContext.Set<T>().AddRange(entities);
        }

        public void UpdateMaria(T entity)
        {
            DbContext.Set<T>().Update(entity);
        }

        public void RemoveMaria(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }

        public void RemoveRangeMaria(IEnumerable<T> entities)
        {
            DbContext.Set<T>().RemoveRange(entities);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return DbContext.Set<T>().FirstOrDefault(expression);
        }

        public T GetById(string Id)
        {
            return DbContext.Set<T>().Find(Id);
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            return DbContext.Set<T>().Any(expression);
        }

        public void RemoveMongo(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void RemoveRangeMongo(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void UpdateMongo(Expression<Func<T, bool>> expression, T entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateManyMongo(List<T> docs, Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
