using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Services
{
    public interface IDeviceService
    {
        (Device result, string message) Store(AddDeviceRequest request);
        (Device result, string message) Update(UpdateDeviceRequest request, string deviceId);
        (Device result, string message) Detail(string Id);
        (Device result, string message) Delete(string Id);
        PaginationResponse<Device> ShowTable(PaginationRequest request);
    }
    public class DeviceService : IDeviceService
    {
        private readonly IRepositoryWrapperMariaDB repositoty;
        private readonly IMapper mapper;
        public DeviceService(IRepositoryWrapperMariaDB repositoty, IMapper mapper)
        {
            this.repositoty = repositoty;
            this.mapper = mapper;
        }

        public (Device result, string message) Delete(string Id)
        {
            (Device result, string message) = Detail(Id);
            if (result is null)
            {
                return (result, message);
            }
            repositoty.Devices.Remove(result);
            repositoty.SaveChanges();
            return (result, "DeleteSuccess");
        }

        public (Device result, string message) Detail(string Id)
        {
            Device device = repositoty.Devices.GetById(Id);
            if (device is null)
            {
                return (null, "DeviceIdNotExist");
            }
            return (device, "Success");
        }

        public PaginationResponse<Device> ShowTable(PaginationRequest request)
        {
            IQueryable<Device> data = repositoty.Devices.FindByCondition(x => String.IsNullOrEmpty(request.SearchContent) || x.DeviceName.ToLower().Contains(request.SearchContent.ToLower()));
            data = SortHelper<Device>.ApplySort(data, request.OrderByQuery);
            PaginationHelper<Device> result = PaginationHelper<Device>.ToPagedList(data, request.PageNumber, request.PageSize);
            PaginationResponse<Device> response = new PaginationResponse<Device>(data, result.PageInfo);
            foreach (Device device in response.PagedData.ToList())
            {
                device.DeviceType = repositoty.DeviceTypes.FindByCondition(x => x.DeviceTypeId.Equals(device.DeviceTypeId)).FirstOrDefault();
            }
            return response;
        }

        public (Device result, string message) Store(AddDeviceRequest request)
        {
            Device device = mapper.Map<Device>(request);
            device.DeviceSettings = JsonConvert.SerializeObject(request.DeviceSettingData);
            device.DevicePassword = request.DevicePassword.MD5Hash();
            Dictionary<string, string> deviceCodes = repositoty.Devices.FindAll().Select(x => x.DeviceCode).ToDictionary(x => x, x => x);
            if (! string.IsNullOrEmpty(request.DeviceCode) && deviceCodes.ContainsKey(request.DeviceCode))
            {
                return (null, "DeviceCodeAlreadyExist");
            }
            if (string.IsNullOrEmpty(request.DeviceCode))
            {
                string code = 6.RandomString();
                while (deviceCodes.ContainsKey(code))
                {
                    code = 6.RandomString();
                }
                device.DeviceCode = code;
            }

            repositoty.Devices.Add(device);
            repositoty.SaveChanges();
            return (device, "Success");
        }

        public (Device result, string message) Update(UpdateDeviceRequest request, string deviceId)
        {
            Device device = repositoty.Devices.GetById(deviceId);
            if (device is null)
            {
                return (null, "DeviceNotExist");
            }
            string passOld = device.DevicePassword;
            device= mapper.Map(request,device);
            
            if( passOld != (request.DevicePassword.MD5Hash()))
            {
                device.DevicePassword = request.DevicePassword.MD5Hash();
            }
            device.DeviceSettings = JsonConvert.SerializeObject(request.DeviceSettingData);

            if (!string.IsNullOrEmpty(request.DeviceCode))
            {
                bool isCode = repositoty.Devices.FindByCondition(x => !x.DeviceId.Equals(deviceId) && x.DeviceCode.Equals(request.DeviceCode)).Any();
                if (isCode)
                {
                    return (null, "DeviceCodeAlreadyExist");
                }
            }
            repositoty.Devices.Update(device);
            repositoty.SaveChanges();
            return (device, "Success");
        }
    }
}
