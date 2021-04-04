using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.App.DesignPatterns.Repositories
{

    public interface IRepositoryRoot<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        T FirstOrDefault(Expression<Func<T, bool>> expression);
        T GetById(string Id);
        bool Any(Expression<Func<T, bool>> expression);

        void Add(T entity);
        void AddRange(IEnumerable<T> entities);

        void UpdateMaria(T entity);

        void UpdateMongo(Expression<Func<T, bool>> expression, T entity);
        void UpdateManyMongo(List<T> docs, Expression<Func<T, bool>> expression);

        void RemoveMaria(T entity);
        void RemoveRangeMaria(IEnumerable<T> entities);

        void RemoveMongo(Expression<Func<T, bool>> expression);
        void RemoveRangeMongo(Expression<Func<T, bool>> expression);
    }
}
