using MongoDB.Bson;
using MongoDB.Driver;
using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Project.App.DesignPatterns.Reponsitories
{
    public interface IRepositoryBaseMongoDB<TEntity>
    {
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        IQueryable<TEntity> FindAll();
        IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression);
        void Remove(FilterDefinition<TEntity> filters);
        void UpdateManyMongo(List<TEntity> docs, Expression<Func<TEntity, bool>> expression);
        void UpdateMongo(Expression<Func<TEntity, bool>> expression, TEntity entity);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression);
        void RemoveRangeMongo(Expression<Func<TEntity, bool>> expression);
        void RemoveMongo(Expression<Func<TEntity, bool>> expression);
    }

    public class RepositoryBaseMongoDB<TEntity> : IRepositoryBaseMongoDB<TEntity> where TEntity : class
    {
        private readonly MongoDBContext _DBContext;
        private IMongoCollection<TEntity> _dbCollection;

        public RepositoryBaseMongoDB(MongoDBContext DBContext)
        {
            _DBContext = DBContext;
            _dbCollection = _DBContext.MongoDatabase.GetCollection<TEntity>(typeof(TEntity).Name);
        }


        public void Add(TEntity entity)
        {
            _dbCollection.InsertOne(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _dbCollection.InsertMany(entities);
        }

        public IQueryable<TEntity> FindAll()
        {
            return _dbCollection.AsQueryable();
        }

        public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression)
        {
            return _dbCollection.AsQueryable().Where(expression);
        }

        public void Remove(FilterDefinition<TEntity> filters)
        {
            _dbCollection.FindOneAndDelete(filters);

        }

        public void UpdateManyMongo(List<TEntity> docs, Expression<Func<TEntity, bool>> expression)
        {

            var updates = new List<WriteModel<TEntity>>();
            foreach (var doc in docs)
            {
                updates.Add(new ReplaceOneModel<TEntity>(expression, doc));
            }
            _dbCollection.BulkWriteAsync(updates);
        }

        public void UpdateMongo(Expression<Func<TEntity, bool>> expression, TEntity entity)
        {
            _dbCollection.ReplaceOne(Builders<TEntity>.Filter.Where(expression), entity);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression)
        {
            return _dbCollection.AsQueryable().FirstOrDefault(expression);
        }

        public void RemoveRangeMongo(Expression<Func<TEntity, bool>> expression)
        {
            _dbCollection.DeleteMany(Builders<TEntity>.Filter.Where(expression));
        }

        public void RemoveMongo(Expression<Func<TEntity, bool>> expression)
        {
            _dbCollection.DeleteOne(expression);
        }

    }
}
