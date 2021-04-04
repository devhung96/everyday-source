using Project.App.Databases;
using Project.Modules.Logs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.DesignPatterns.Reponsitories
{
    public interface IRepositoryWrapperMongoDB
    {
        IRepositoryBaseMongoDB<Log> Logs { get; }
        IRepositoryBaseMongoDB<AppLog> AppLogs { get; }
    }

    public class RepositoryWrapperMongoDB : IRepositoryWrapperMongoDB
    {
        private readonly MongoDBContext _mongoDBContext;
        private IRepositoryBaseMongoDB<Log> logs;
        private IRepositoryBaseMongoDB<AppLog> appLogs;
        public RepositoryWrapperMongoDB(MongoDBContext mongoDBContext)
        {
            _mongoDBContext = mongoDBContext;
        }

        public IRepositoryBaseMongoDB<Log> Logs
        {
            get
            {
                return logs ?? (logs = new RepositoryBaseMongoDB<Log>(_mongoDBContext));
            }
        }

        public IRepositoryBaseMongoDB<AppLog> AppLogs => appLogs ?? new RepositoryBaseMongoDB<AppLog>(_mongoDBContext); 
    }
}
