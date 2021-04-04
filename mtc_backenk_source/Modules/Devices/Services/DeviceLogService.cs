using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Requests;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Services
{
    public interface IDeviceLogService
    {
        object LogUpdate(UpdateDevice valueInput, Device device, string userId);
        object LogCreate(Device device, string userId);
        object LogDelete(Device device, string userId);
        (PaginationResponse<ResponseDeviceLog> devices, string message) ShowAll(PaginationRequest requestTable);
        (PaginationResponse<ResponseDeviceLog>, string) ShowAllByDevice(PaginationRequest requestTable, string DeviceId);

    }
    public class DeviceLogService : IDeviceLogService
    {
        private readonly IRepositoryWrapperMongoDB repositoryWrapperMongoDB;
        private readonly IRepositoryWrapperMariaDB repositoryWrapperMariaDB;

        public DeviceLogService(IRepositoryWrapperMongoDB _repositoryWrapperMongoDB, IRepositoryWrapperMariaDB _repositoryWrapperMariaDB)
        {
            repositoryWrapperMongoDB = _repositoryWrapperMongoDB;
            repositoryWrapperMariaDB = _repositoryWrapperMariaDB;
        }
        public object LogCreate(Device device, string userId)
        {
            List<object> values = new List<object>();

            DeviceLog deviceLog = new DeviceLog()
            {
                DeviceLogValue = JsonConvert.SerializeObject(values),
                DeviceLogType = (int)ENUMDEVICELOGTYPE.CREATE,
                UserId = userId,
                DeviceLogCreatedAt = DateTime.Now,
                DeviceId = device.DeviceId,
            };
            repositoryWrapperMongoDB.DeviceLogs.Add(deviceLog);
            return deviceLog;
        }

        public object LogDelete(Device device, string userId)
        {
            List<object> values = new List<object>();
            DeviceLog deviceLog = new DeviceLog()
            {
                DeviceLogValue = JsonConvert.SerializeObject(values),
                DeviceLogType = (int)ENUMDEVICELOGTYPE.DELETE,
                UserId = userId,
                DeviceLogCreatedAt = DateTime.Now,
                DeviceId = device.DeviceId,
            };
            repositoryWrapperMongoDB.DeviceLogs.Add(deviceLog);
            return deviceLog;
        }

        public object LogUpdate(UpdateDevice valueInput, Device device, string userId)
        {
            List<object> values = new List<object>();
            if(valueInput.DeviceWarrantyExpiresDate != null)
            {
                values = new List<object>()
                {
                    new 
                    {
                        field = "WarrantyExpiresDate",
                        NewValue = valueInput.DeviceWarrantyExpiresDate,
                        OldValue = device.DeviceWarrantyExpiresDate

                    }

                };
            }
            if(valueInput.DeviceExpired != null)
            {
                values = new List<object>()
                {
                    new
                    {
                        field = "license",
                        NewValue = valueInput.DeviceExpired,
                        OldValue = device.DeviceExpired

                    }

                };
            }
            DeviceLog deviceLog = new DeviceLog()
            {
                DeviceLogValue = JsonConvert.SerializeObject(values),
                DeviceLogType = (int)ENUMDEVICELOGTYPE.UPDATE,
                UserId = userId,
                DeviceLogCreatedAt = DateTime.Now,
                DeviceId = device.DeviceId
            };
            repositoryWrapperMongoDB.DeviceLogs.Add(deviceLog);
            return deviceLog;
        }
        public (PaginationResponse<ResponseDeviceLog> devices, string message) ShowAll(PaginationRequest requestTable)
        {
            IQueryable<DeviceLog> deviceLogs = repositoryWrapperMongoDB.DeviceLogs
                .FindAll().OrderByDescending(x => x.DeviceLogCreatedAt);
            //deviceLogs = deviceLogs.Where(x =>
            //  string.IsNullOrEmpty(requestTable.SearchContent) ||
            //  (!string.IsNullOrEmpty(requestTable.SearchContent) &&
            //      (
            //          (x.DeviceLogType.Equals(requestTable.SearchContent))
            //      )
            //  ));
            PaginationHelper<DeviceLog> deviceLogInfo = PaginationHelper<DeviceLog>.ToPagedList(deviceLogs, requestTable.PageNumber, requestTable.PageSize);
            PaginationResponse<ResponseDeviceLog> paginationResponse = new PaginationResponse<ResponseDeviceLog>(deviceLogInfo.Select(x => new ResponseDeviceLog(x, GetUser(x.UserId), GetDevice(x.DeviceId))), deviceLogInfo.PageInfo);
            return (paginationResponse, "ShowAllSuccess");
        }

        public (PaginationResponse<ResponseDeviceLog>, string) ShowAllByDevice(PaginationRequest requestTable, string DeviceId)
        {
            IQueryable<DeviceLog> deviceLogs = repositoryWrapperMongoDB.DeviceLogs
                .FindByCondition(x => x.DeviceId.Equals(DeviceId)).OrderByDescending(x => x.DeviceLogCreatedAt);
            PaginationHelper<DeviceLog> deviceLogInfo = PaginationHelper<DeviceLog>.ToPagedList(deviceLogs, requestTable.PageNumber, requestTable.PageSize);
            PaginationResponse<ResponseDeviceLog> paginationResponse = new PaginationResponse<ResponseDeviceLog>(deviceLogInfo.Select(x => new ResponseDeviceLog(x, GetUser(x.UserId), GetDevice(x.DeviceId))), deviceLogInfo.PageInfo);
            return (paginationResponse, "ShowAllSuccess");
        }

        public string UserName(string userId)
        {
            return repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault()?.UserName;
        }
        public Device GetDevice(string deviceId)
        {
            return repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId.Equals(deviceId)).FirstOrDefault();
        }
        public string UserAvatar(string userId)
        {
            return repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault()?.UserName;
        }
        public User GetUser (string userId)
        {
            return repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault();
        }
    }
}
