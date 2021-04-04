using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository
{
    public interface IRepositoryMongoBase<T>
    {
        long CountData(Expression<Func<T, bool>> expression);
        IQueryable<T> FindAll(int page = 0, int limit = 0);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, int page = 0, int limit = 0);
        IQueryable<T> GetOrderBy(Expression<Func<T, bool>> expression, int page = 0, int limit = 0, string sortType = "", string sortField = "");
        void Add(T entity);
        void AddRange(IEnumerable<T> entity);
        void Update(T entity, Expression<Func<T, bool>> expression);
        void Remove(Expression<Func<T, bool>> expression);
        T FirstOrDefault(Expression<Func<T, bool>> expression);
        Task InsertOneAsync(T document);
        public void UpdateMany(List<T> docs, Expression<Func<T, bool>> expression);
        IQueryable<T> FindByCondition(FilterDefinition<T> filters = null, int page = 0, int limit = 0);
        Task<T> FindByIdAsync(string id);
        IEnumerable<T> FindConitionBuilder(
           FilterDefinition<T> filters = null);

        Task DeleteOneAsync(IClientSessionHandle sessionHandle, Expression<Func<T, bool>> filterExpression);
        Task DeleteManyAsync(FilterDefinition<T> filter);
        Task<IClientSessionHandle> Session();

        Task DeleteManyAsync(IClientSessionHandle sessionHandle, FilterDefinition<T> filter);
    }

    public class RepositoryMongoBase<T> : IRepositoryMongoBase<T> where T : class
    {
        private readonly IMongoDatabase Database;
        public readonly IMongoClient mongoClient;

        public RepositoryMongoBase(MongoDBContext context)
        {
            Database = context.MongoDatabase;
            mongoClient = Database.Client;
        }

        public IQueryable<T> FindAll(int page = 0, int limit = 0)
        {
            if(page > 0 && limit > 0)
            {
                return Database.GetCollection<T>(typeof(T).Name).Find(_ => true).Limit(limit).Skip((page - 1) * limit).ToList().AsQueryable();
            }
                
            return Database.GetCollection<T>(typeof(T).Name).AsQueryable().AsNoTracking();
        }
        public virtual Task InsertOneAsync(T document)
        {
            return Task.Run(() => Database.GetCollection<T>(typeof(T).Name).InsertOneAsync(document));
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, int page = 0, int limit = 0)
        {
            expression ??= x => true;
            if (page > 0 && limit > 0)
            {
                return Database.GetCollection<T>(typeof(T).Name).Find(expression).Limit(limit).Skip((page - 1) * limit).ToEnumerable().AsQueryable();
            }
            return Database.GetCollection<T>(typeof(T).Name).Find(expression).ToEnumerable().AsQueryable();
        }
        public IQueryable<T> FindByCondition(FilterDefinition<T> filters = null, int page = 0, int limit = 0)
        {
            if (page > 0 && limit > 0)
            {
                return Database.GetCollection<T>(typeof(T).Name).Find(filters).Limit(limit).Skip((page - 1) * limit).ToEnumerable().AsQueryable();
            }
            return Database.GetCollection<T>(typeof(T).Name).Find(filters).ToEnumerable().AsQueryable();
        }

        public virtual IEnumerable<T> FindConitionBuilder(
           FilterDefinition<T> filters = null)
        {
            return Database.GetCollection<T>(typeof(T).Name).Find(filters).ToEnumerable();
        }

        public IQueryable<T> GetOrderBy(Expression<Func<T, bool>> expression, int page = 0, int limit = 0, string sortType = "", string sortField = "")
        {
            if (page > 0 && limit > 0)
            {
                if(sortType == "asc")
                {
                    return Database.GetCollection<T>(typeof(T).Name).Find(expression)
                        .Sort(Builders<T>.Sort.Ascending(sortField))
                        .Limit(limit).Skip((page - 1) * limit).ToEnumerable().AsQueryable();
                }
                else
                {
                    return Database.GetCollection<T>(typeof(T).Name).Find(expression)
                    .Sort(Builders<T>.Sort.Descending(sortField))
                    .Limit(limit).Skip((page - 1) * limit).ToEnumerable().AsQueryable();
                }
            }
            else
            {
                if (sortType == "asc")
                {
                    return Database.GetCollection<T>(typeof(T).Name).Find(expression)
                        .Sort(Builders<T>.Sort.Ascending(sortField))
                        .ToEnumerable().AsQueryable();
                }
                else
                {
                    return Database.GetCollection<T>(typeof(T).Name).Find(expression)
                    .Sort(Builders<T>.Sort.Descending(sortField))
                    .ToEnumerable().AsQueryable();
                }
            }
        }

        [Obsolete]
        public long CountData(Expression<Func<T, bool>> expression)
        {
            return Database.GetCollection<T>(typeof(T).Name).Find(expression).Count();
        }

        private static FilterDefinition<T> FilterId(string key)
        {
            return Builders<T>.Filter.Eq("_id", key);
        }
        public virtual Task<T> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                return Database.GetCollection<T>(typeof(T).Name).Find(FilterId(id)).SingleOrDefaultAsync();
            });
        }
        public T FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            return Database.GetCollection<T>(typeof(T).Name).AsQueryable().Where(expression).FirstOrDefault();
        }

        public void Add(T entity)
        {
            Database.GetCollection<T>(typeof(T).Name).InsertOne(entity);
        }

        public void AddRange(IEnumerable<T> entity)
        {
            Database.GetCollection<T>(typeof(T).Name).InsertMany(entity);
        }

        public void Update(T entity, Expression<Func<T, bool>> expression)
        {
            expression ??= x => true;
            Database.GetCollection<T>(typeof(T).Name).ReplaceOne(expression, entity);
        }


        public void UpdateMany(List<T> docs, Expression<Func<T, bool>> expression)
        {

            var updates = new List<WriteModel<T>>();
            foreach (var doc in docs)
            {
                updates.Add(new ReplaceOneModel<T>(expression, doc));
            }
            Database.GetCollection<T>(typeof(T).Name).BulkWriteAsync(updates);
        }




        public void Remove(Expression<Func<T, bool>> expression)
        {
            Database.GetCollection<T>(typeof(T).Name).DeleteOne(expression);
        }
        public Task DeleteManyAsync(FilterDefinition<T> filter)
        {
            return Task.Run(() => Database.GetCollection<T>(typeof(T).Name).DeleteManyAsync(filter));
        }

        public Task DeleteOneAsync(IClientSessionHandle sessionHandle, Expression<Func<T, bool>> filterExpression)
        {
            return Task.Run(() => Database.GetCollection<T>(typeof(T).Name).FindOneAndDeleteAsync(sessionHandle, filterExpression));
        }
        
        public Task DeleteManyAsync(IClientSessionHandle sessionHandle, FilterDefinition<T> filter)
        {
            return Task.Run(() => Database.GetCollection<T>(typeof(T).Name).DeleteManyAsync(sessionHandle, filter));
        }
        public async Task<IClientSessionHandle> Session()
        {
            return await mongoClient.StartSessionAsync();
        }
    }
}
