using Project.App.Databases;
using Project.Modules.Devices.Entities;
using Project.Modules.Groups.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.DesignPatterns.Reponsitories
{
    public interface IRepositoryWrapperMongoDB
    {
        IRepositoryBaseMongoDB<DeviceLog> DeviceLogs { get; }
        IRepositoryBaseMongoDB<GroupHistory> GroupHistories { get; }
    }

    public class RepositoryWrapperMongoDB : IRepositoryWrapperMongoDB
    {
        private readonly MongoDBContext _mongoDBContext;
        private readonly RepositoryBaseMongoDB<DeviceLog> deviceLogs;
        private readonly RepositoryBaseMongoDB<GroupHistory> groupHistories;
        public RepositoryWrapperMongoDB(MongoDBContext mongoDBContext)
        {
            _mongoDBContext = mongoDBContext;
        }

        public IRepositoryBaseMongoDB<DeviceLog> DeviceLogs => deviceLogs ?? new RepositoryBaseMongoDB<DeviceLog>(_mongoDBContext);
        public IRepositoryBaseMongoDB<GroupHistory> GroupHistories => groupHistories?? new RepositoryBaseMongoDB<GroupHistory>(_mongoDBContext);
    }
}
