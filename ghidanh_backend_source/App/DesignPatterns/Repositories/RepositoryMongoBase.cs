using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Project.App.DesignPatterns.Repositories
{
    public class RepositoryMongoBase<T> : IRepositoryRoot<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;
        private readonly MongoDBContext DBContext;



        public RepositoryMongoBase(MongoDBContext DBContext)
        {
            this.DBContext = DBContext;
            _collection = DBContext.MongoDatabase.GetCollection<T>(typeof(T).Name);

        }

        public void Add(T entity)
        {
            _collection.InsertOne(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _collection.InsertMany(entities);
        }

        public IQueryable<T> FindAll()
        {
            return _collection.AsQueryable().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().Where(expression).AsNoTracking();
        }

        public void Remove(FilterDefinition<T> filters)
        {
            _collection.FindOneAndDelete(filters);

        }

        public void UpdateManyMongo(List<T> docs, Expression<Func<T, bool>> expression)
        {

            var updates = new List<WriteModel<T>>();
            foreach (var doc in docs)
            {
                updates.Add(new ReplaceOneModel<T>(expression, doc));
            }
            _collection.BulkWriteAsync(updates);
        }


        public void UpdateMongo(Expression<Func<T, bool>> expression, T entity)
        {
            _collection.ReplaceOne(Builders<T>.Filter.Where(expression), entity);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().FirstOrDefault(expression);
        }


        public T GetById(string Id)
        {
            throw new NotImplementedException();
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void RemoveRangeMongo(Expression<Func<T, bool>> expression)
        {
            _collection.DeleteMany(Builders<T>.Filter.Where(expression));
        }
        public void RemoveMongo(Expression<Func<T, bool>> expression)
        {
            _collection.DeleteOne(expression);
        }

        public void UpdateMaria(T entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveMaria(T entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRangeMaria(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }



}
