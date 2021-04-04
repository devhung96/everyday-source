using AutoMapper;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Logs.Entities;
using Project.Modules.Logs.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Logs.Services
{
    public interface IAppLogService
    {
        AppLog Create(AppLogRequest request);
        List<AppLog> ShowAll();
        AppLog Detail(string key);

    }
    public class AppLogService : IAppLogService
    {
        private readonly IRepositoryWrapperMongoDB repository;
        private readonly IMapper mapper;
        public AppLogService(IRepositoryWrapperMongoDB repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public AppLog Create(AppLogRequest request)
        {
            AppLog appLog = mapper.Map<AppLog>(request);
            repository.AppLogs.Add(appLog);
            return (appLog);
        }

        public AppLog Detail(string key)
        {
            AppLog appLog = repository.AppLogs.FindByCondition(x => x.Key.Equals(key)).FirstOrDefault();
            return (appLog);
        }

        public List<AppLog> ShowAll()
        {
            List<AppLog> appLogs = repository.AppLogs.FindAll().ToList();
            return appLogs;
        }
    }
}
