using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.Logs.Entities;
using Project.Modules.Logs.Models;
using Project.Modules.Logs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Logs.Services
{
    public interface ILogService
    {
        void Store(StoreLogRequest request);
        PaginationResponse<ResponseLog> GetAllLog(PaginationRequest request);
    }

    public class LogService : ILogService
    {
        private readonly IRepositoryWrapperMongoDB RepositoryWrapperMongo;
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMaria;
        public LogService(IRepositoryWrapperMongoDB repositoryWrapperMongo, IRepositoryWrapperMariaDB repositoryWrapperMaria)
        {
            RepositoryWrapperMongo = repositoryWrapperMongo;
            RepositoryWrapperMaria = repositoryWrapperMaria;
        }

        public void Store(StoreLogRequest request)
        {
            Log log = new Log
            {
                DeviceId = request.DeviceId,
                LogName = request.LogName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                LogRegion = "Viet Nam",
                LogStatus = request.LogStatus,
                UserId = request.UserId,
                LogMessage = request.LogMessage,
                LogAccess = request.LogAccess,
                LogAccessTime = request.LogAccessTime
            };
            RepositoryWrapperMongo.Logs.Add(log);
        }

        public PaginationResponse<ResponseLog> GetAllLog(PaginationRequest request)
        {
            List<ResponseLog> responseLogs = new List<ResponseLog>();
            var prepareLogs = RepositoryWrapperMongo.Logs.FindAll().OrderByDescending(x => x.LogCreatedAt);

            var data = SortHelper<Log>.ApplySort(prepareLogs, request.OrderByQuery);
            PaginationHelper<Log> result = PaginationHelper<Log>.ToPagedList(data, request.PageNumber, request.PageSize);



            List<Device> allDevices = RepositoryWrapperMaria.Devices.FindAll().ToList();
            List<DeviceType> allDeviceTypes = RepositoryWrapperMaria.DeviceTypes.FindByCondition(x => allDevices.Select(x => x.DeviceTypeId).Contains(x.DeviceTypeId)).ToList();

            foreach (var log in result)
            {
                Device device = allDevices.FirstOrDefault(x => x.DeviceId.Equals(log.DeviceId));
                DeviceType deviceType = allDeviceTypes.FirstOrDefault(x => device != null && x.DeviceTypeId.Equals(device.DeviceTypeId));

                ResponseLog responseLog = new ResponseLog
                {
                    LogName = log.LogName,
                    LogAccess = log.LogAccess,
                    LogAccessTime = log.LogAccessTime,
                    LogRegion = log.LogRegion,
                    UserId = log.UserId,
                    LastName = log.LastName,
                    FirstName = log.FirstName,
                    LogMessage = log.LogMessage,
                    LogStatus = log.LogStatus,
                    DeviceName = device?.DeviceName,
                    DeviceType = deviceType?.DeviceTypeName
                };
                responseLogs.Add(responseLog);
            }
          
            PaginationResponse<ResponseLog> response = new PaginationResponse<ResponseLog>(responseLogs, result.PageInfo);
            return response;
        }
    }
}
