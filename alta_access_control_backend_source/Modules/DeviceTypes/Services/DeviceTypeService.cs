using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.DeviceTypes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeviceTypes.Services
{
    public interface IDeviceTypeService
    {
        (DeviceType DeviceType, string message) Detail(string id);
        PaginationResponse<DeviceType> ShowTable(PaginationRequest request);
        List<DeviceType> ShowAll();
    }
    public class DeviceTypeService : IDeviceTypeService
    {
        private readonly IRepositoryWrapperMariaDB repository;
        public DeviceTypeService(IRepositoryWrapperMariaDB repository)
        {
            this.repository = repository;
        }
        public (DeviceType DeviceType, string message) Detail(string id)
        {
            DeviceType deviceType = repository.DeviceTypes.GetById(id);
            if(deviceType is null)
            {
                return (null, "DeviceTypeNotExist");
            }
            return (deviceType, "Success");
        }

        public List<DeviceType> ShowAll()
        {
            return repository.DeviceTypes.FindAll().ToList(); 
        }

        public PaginationResponse<DeviceType> ShowTable(PaginationRequest request)
        {
            IQueryable<DeviceType> data = repository.DeviceTypes.FindByCondition(x => string.IsNullOrEmpty(request.SearchContent) || x.DeviceTypeName.ToLower().Contains(request.SearchContent.ToLower()));
            PaginationHelper<DeviceType> result = PaginationHelper<DeviceType>.ToPagedList(data, request.PageNumber, request.PageSize);
            PaginationResponse<DeviceType> respone = new PaginationResponse<DeviceType>(data, result.PageInfo);
            return respone;
        }
    }
}
