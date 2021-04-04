using AutoMapper;
using Microsoft.AspNetCore.Http;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.DeviceTypes.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeviceTypes.Services
{
    public interface IDeviceTypeServices
    {
        (DeviceType deviceType, string message) Store(DeviceType deviceType);
        (bool result, string message) Delete(string DeviceTypeID, string url);
        (DeviceType deviceType, string message) FindID(string deviceTypeID, string url);
        (DeviceType deviceType, string message) Update(string deviceTypeID, UpdateDeviceTypeRequest request);
        (IQueryable<DeviceType> deviceTypes, string message) ShowAll(string url);
    }
    public class DeviceTypeServices : IDeviceTypeServices
    {
        private readonly IRepositoryWrapperMariaDB repositoryWrapperMariaDB;
        private readonly IMapper mapper;
        public DeviceTypeServices(IRepositoryWrapperMariaDB _repositoryWrapperMariaDB, IMapper _mapper)
        {
            repositoryWrapperMariaDB = _repositoryWrapperMariaDB;
            mapper = _mapper;
        }

        public (DeviceType deviceType, string message) Store(DeviceType deviceType)
        {
            repositoryWrapperMariaDB.DeviceTypes.Add(deviceType);
            repositoryWrapperMariaDB.SaveChanges();
            return (deviceType, "CreatedDeviceTypeSuccess");
        }

        public (bool result, string message) Delete(string deviceTypeId, string url)
        {
            DeviceType deviceType = repositoryWrapperMariaDB.DeviceTypes.FindByCondition(f => f.DeviceTypeId == deviceTypeId).FirstOrDefault();
            if (deviceType is null)
            {
                return (false, "DeviceTypeNotFound");
            }
            try
            {
                deviceType.TypeStatus = DeviceTypeStatus.DEACTIVATED;
                string path = deviceType.DeviceTypeIcon;
                if (path != null)
                {
                    string mediaUri = GeneralHelper.UrlCombine(url, deviceType.DeviceTypeIcon);
                    string fileName = GeneralHelper.GetFileName(mediaUri);
                    var didretoryVideo = GeneralHelper.UrlCombine(GeneralHelper.GetDirectoryFromFile(mediaUri), fileName);
                    bool checkExists = System.IO.File.Exists(didretoryVideo);
                    if (checkExists)
                    {
                        (bool check, string message) = GeneralHelper.DeleteFile(path);
                        if (!check)
                        {
                            return (false, message);
                        }
                    }
                }
                deviceType.DeviceTypeIcon = null;
                repositoryWrapperMariaDB.DeviceTypes.Remove(deviceType);
                repositoryWrapperMariaDB.SaveChanges();
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

            return (true, "DeleteDeviceTypeSuccess");
        }

        public (DeviceType deviceType, string message) FindID(string deviceTypeId, string url)
        {
            DeviceType result = repositoryWrapperMariaDB.DeviceTypes.FindByCondition(f => f.DeviceTypeId == deviceTypeId).FirstOrDefault();
            if (result is null)
            {
                return (null, "DeviceTypeNotFound");
            }
            if (result.DeviceTypeIcon != null)
            {
                result.DeviceTypeIcon = GeneralHelper.UrlCombine(url, result.DeviceTypeIcon);
            }

            return (result, "GetDeviceTypeSuccess");
        }

        public (DeviceType deviceType, string message) Update(string deviceTypeId, UpdateDeviceTypeRequest request)
        {
            DeviceType deviceType = repositoryWrapperMariaDB.DeviceTypes.FindByCondition(f => f.DeviceTypeId == deviceTypeId).FirstOrDefault();
            if (deviceType is null)
            {
                return (null, "DeviceTypeNotFound");
            }
            string deviceTypeIcon = deviceType.DeviceTypeIcon;
            if (request.typeIcon != null)
            {
                (string fileName, _) = GeneralHelper.UploadFileV2(request.typeIcon, "deviceTypes").Result;
                deviceTypeIcon = GeneralHelper.UrlCombine("deviceTypes", fileName);
            }

            deviceType.DeviceTypeName = request.DeviceTypeName;
            deviceType.DeviceTypeIcon = deviceTypeIcon;
            deviceType.DeviceTypeComment = request.DeviceTypeComment;
            repositoryWrapperMariaDB.DeviceTypes.Update(deviceType);
            repositoryWrapperMariaDB.SaveChanges();

            return (deviceType, "UpdateTypeDeviceSuccess");
        }

        public (IQueryable<DeviceType> deviceTypes, string message) ShowAll(string url)
        {
            IQueryable<DeviceType> devices = repositoryWrapperMariaDB.DeviceTypes.FindAll()
                 .Select(x => new DeviceType
                 {
                     DeviceTypeIcon = x.DeviceTypeIcon != null ? GeneralHelper.UrlCombine(url, x.DeviceTypeIcon) : x.DeviceTypeIcon,
                     DeviceTypeComment = x.DeviceTypeComment,
                     CreatedAt = x.CreatedAt,
                     DeviceTypeId = x.DeviceTypeId,
                     DeviceTypeName = x.DeviceTypeName,
                     PlaylistDefault = x.PlaylistDefault,
                     TypeStatus = x.TypeStatus
                 });
            return (devices, "ShowAllSuccess");
        }
    }
}
