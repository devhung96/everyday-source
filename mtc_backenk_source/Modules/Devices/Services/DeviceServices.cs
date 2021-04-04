using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.App.Mqtt;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Requests;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Settings.Entitites;
using Project.Modules.Settings.Services;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Services
{

    public interface IDeviceServices
    {
        (Device result, string message) Store(StoreDevice storeDevice, string userId, string imageName = null);
        (Device device, string message) FindID(string deviceId);
        (Device result, string message) Delete(string deviceId, string userId);
        (object result, string message) DeleteMulti(DeleteMultiDeviceRequest valueInput, string userId);
        (PaginationResponse<DeviceResponse> devices, string message) ShowAllPagination(FilterDeviceRequest requestTable, string groupId, string url = null);
        (PaginationResponse<DeviceResponse> devices, string message) ShowAllByPagination(FilterDeviceRequest requestTable, string groupId, string url = null);
        (object data, string message) Login(LoginDeviceRequest request);
        (Device device, string message) Update(string deviceId, UpdateDevice request, string userId, string url = null);
        (bool result, string message) Logout(string token);
        (bool result, string message) LogoutCms(string deviceId);
        (List<Device> devices, string message) ShowWithType(string typeDevice);
        (Device device, string message) UpdateMemory(string deviceId, UpdateMemory request);
        (Device device, string message) Active(string deviceId);
        (Device device, string message) CheckTokenDevice(string token);
        (int? total, int? totalActive, int? totalDeactive, int? totalWarranty) Dashboard(string url, string userId);
        (Device, string) DeleteMemory(string deviceId);
        (DeviceResponse, string) ChangeLockDevice(ChangeLockDeviceRequest valueInput, string deviceId, string url);
        (bool, string) ChangeLockMultiDevice(List<ChangeLockMultiDeviceRequest> valueInput);
        (DeviceResponse, string) ChangePowerDevice(string deviceId, int value);
        (bool data, string message) DeleteMultiMemory(List<string> deviceId);
        (List<Device>, string) ChangeActiveMultiDevice(List<ChangeActiveMultiRequest> valueInput);
        (List<DeviceResponse>, string) RestartMultiDevice(RestartDeviceRequest valueInput);
    }
    public class DeviceServices : IDeviceServices
    {
        private readonly IConfiguration configuration;
        private readonly IRepositoryWrapperMariaDB repositoryWrapperMariaDB;
        private readonly IMapper mapper;
        private readonly IDeviceLogService deviceLogService;
        private readonly IMqttSingletonService MqttSingletonService;
        private readonly ISettingService settingService;
        public DeviceServices(ISettingService _settingService, IRepositoryWrapperMariaDB _repositoryWrapperMariaDB, IConfiguration _configuration, IMapper _mapper, IDeviceLogService _deviceLogService, IMqttSingletonService _MqttSingletonService)
        {
            repositoryWrapperMariaDB = _repositoryWrapperMariaDB;
            configuration = _configuration;
            mapper = _mapper;
            deviceLogService = _deviceLogService;
            MqttSingletonService = _MqttSingletonService;
            settingService = _settingService;
        }
        public (Device result, string message) Store(StoreDevice storeDevice, string UserId, string imageName = null)
        {
            string saft = 6.RandomString();
            string group_id = "";
            User user = repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(UserId)).FirstOrDefault();
            if (user is null)
            {
                return (null, "UserIdNotFound");
            }
            if (user.GroupId != null)
            {
                group_id = user.GroupId;
            }
            else
            {
                if (string.IsNullOrEmpty(storeDevice.GroupId))
                {
                    return (null, "GroupIdIsRequired");
                }
                group_id = storeDevice.GroupId;
            }
            bool checkSku = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceSku.Equals(storeDevice.DeviceSku)).Any();
            if (checkSku)
            {
                return (null, "SKUExists");
            }
            Device device = mapper.Map(storeDevice, new Device());
            device.DevicePhoto = imageName;
            device.UserId = UserId;
            device.GroupId = group_id;
            device.ColorCode = GeneralHelper.RandomColor();
            device.DevicePass = (device.DevicePass + saft).HashPassword();
            device.DeviceSaft = saft;
            repositoryWrapperMariaDB.Devices.Add(device);
            repositoryWrapperMariaDB.SaveChanges();
            deviceLogService.LogCreate(device, UserId);
            return (device, "StoreDeviceSuccess");
        }

        public (Device device, string message) Active(string deviceId)
        {
            Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId == deviceId).FirstOrDefault();
            if (device is null)
            {
                return (null, "DeviceNotFound");
            }
            if (device.DeviceStatus == DEVICESTATUS.DEACTIVATED)
            {
                device.DeviceStatus = DEVICESTATUS.ACTIVED;
                Console.WriteLine($"Topic : {TopicDefine.STATUS_DEVICE}/{device.DeviceId}************body : {JsonConvert.SerializeObject(new { status = device.DeviceStatus })}");
                MqttSingletonService.PingMessage($"{TopicDefine.STATUS_DEVICE}/{device.DeviceId}",JsonConvert.SerializeObject(new { status = device.DeviceStatus}),true);
            }
            else
            {
                device.DeviceStatus = DEVICESTATUS.DEACTIVATED;
                Console.WriteLine($"Topic : {TopicDefine.STATUS_DEVICE}/{device.DeviceId}************body : {JsonConvert.SerializeObject(new { status = device.DeviceStatus })}");
                MqttSingletonService.PingMessage($"{TopicDefine.STATUS_DEVICE}/{device.DeviceId}", JsonConvert.SerializeObject(new { status = device.DeviceStatus }), true);
            }
            repositoryWrapperMariaDB.Devices.Update(device);
            repositoryWrapperMariaDB.SaveChanges();
            return (device, "ActiveSuccess");
        }
        public (List<Device>, string) ChangeActiveMultiDevice(List<ChangeActiveMultiRequest> valueInput)
        {
            IDbContextTransaction dbContextTransaction = repositoryWrapperMariaDB.BeginTransaction();
            List<Device> devices = new List<Device>();
            foreach (ChangeActiveMultiRequest data in valueInput)
            {
                Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId == data.DeviceId).FirstOrDefault();
                if (device is null)
                {
                    dbContextTransaction.Rollback();
                    return (null, "DeviceNotFound");
                }
                device.DeviceStatus = data.DeviceStatus;
                repositoryWrapperMariaDB.Devices.Update(device);
                repositoryWrapperMariaDB.SaveChanges();
                devices.Add(device);
                Console.WriteLine($"Topic : {TopicDefine.STATUS_DEVICE}/{device.DeviceId}************body : {JsonConvert.SerializeObject(new { status = device.DeviceStatus })}");
                MqttSingletonService.PingMessage($"{TopicDefine.STATUS_DEVICE}/{device.DeviceId}", JsonConvert.SerializeObject(new { status = device.DeviceStatus }), true);
            }
            dbContextTransaction.Commit();
            return (devices, "ChangeActiveMultiDeviceSuccess");
        }
        public (Device device, string message) FindID(string deviceId)
        {
            Device result = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId == deviceId)
                .Include(x => x.User)
                .Include(x => x.DeviceTokens)
                .Include(x => x.DeviceType)
                .FirstOrDefault();
            if (result is null)
            {
                return (null, "DeviceNotFound");
            }
            return (result, "GetDetailSuccess");
        }

        public (Device result, string message) Delete(string deviceId, string userId)
        {
            Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId == deviceId).FirstOrDefault();
            if (device is null)
            {
                return (null, "DeviceNotFound");
            }
            repositoryWrapperMariaDB.Devices.Remove(device);
            repositoryWrapperMariaDB.SaveChanges();
            deviceLogService.LogDelete(device, userId);
            return (device, "DeleteSuccess");
        }
        public (object result, string message) DeleteMulti(DeleteMultiDeviceRequest valueInput, string userId)
        {
            IDbContextTransaction dbContextTransaction = repositoryWrapperMariaDB.BeginTransaction();
            List<Device> devices = new List<Device>();
            foreach (string deviceId in valueInput.DeviceIds)
            {
                Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId == deviceId).FirstOrDefault();
                if (device is null)
                {
                    dbContextTransaction.Rollback();
                    return (null, "DeviceNotFound");
                }
                repositoryWrapperMariaDB.Devices.Remove(device);
                repositoryWrapperMariaDB.SaveChanges();
                deviceLogService.LogDelete(device, userId);
                devices.Add(device);
            }
            dbContextTransaction.Commit();
            return (devices, "DeleteSuccess");


        }
        public IQueryable<Device> ShowAll()
        {
            IQueryable<Device> devicesList = repositoryWrapperMariaDB.Devices
                .FindAll()
                .Include(x => x.User)
                .Include(x => x.DeviceTokens)
                .Include(x => x.DeviceType)
                .OrderByDescending(x => x.CreatedAt);
            return devicesList;
        }
        public (PaginationResponse<DeviceResponse> devices, string message) ShowAllPagination(FilterDeviceRequest requestTable, string groupId, string url = null)
        {
            IQueryable<Device> devicesList = ShowAll();
            devicesList = devicesList.Where(x =>
              string.IsNullOrEmpty(requestTable.SearchContent) ||
              (!string.IsNullOrEmpty(requestTable.SearchContent) &&
                  (
                      (!string.IsNullOrEmpty(x.DeviceName) && x.DeviceName.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                      (!string.IsNullOrEmpty(x.LoginName) && x.LoginName.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                      (!string.IsNullOrEmpty(x.DeviceMACAddress) && x.DeviceMACAddress.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                      (!string.IsNullOrEmpty(x.DeviceSku) && x.DeviceSku.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                      (!string.IsNullOrEmpty(x.DeviceLocation) && x.DeviceLocation.ToLower().Contains(requestTable.SearchContent.ToLower()))
                  )
              ));
            devicesList = devicesList.Where(x => requestTable.DeviceStatus == null || x.DeviceStatus == requestTable.DeviceStatus);
            if (!string.IsNullOrEmpty(groupId))
            {
                devicesList = devicesList.Where(x => x.GroupId == groupId);
            }
            PaginationHelper<Device> deviceInfo = PaginationHelper<Device>.ToPagedList(devicesList, requestTable.PageNumber, requestTable.PageSize);
            PaginationResponse<DeviceResponse> paginationResponse = new PaginationResponse<DeviceResponse>(deviceInfo.Select(x => new DeviceResponse(x, url, getUserGroup(x.GroupId))), deviceInfo.PageInfo);
            return (paginationResponse, "ShowAllSuccess");
        }
        public (PaginationResponse<DeviceResponse> devices, string message) ShowAllByPagination(FilterDeviceRequest requestTable, string groupId, string url = null)
        {
            IQueryable<Device> devicesList = ShowAll().Where(x => x.User.GroupId == groupId);
            devicesList = devicesList.Where(x =>
              string.IsNullOrEmpty(requestTable.SearchContent) ||
              (!string.IsNullOrEmpty(requestTable.SearchContent) &&
                  (
                      (!string.IsNullOrEmpty(x.DeviceName) && x.DeviceName.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                      (!string.IsNullOrEmpty(x.LoginName) && x.LoginName.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                      (!string.IsNullOrEmpty(x.DeviceMACAddress) && x.DeviceMACAddress.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                      (!string.IsNullOrEmpty(x.DeviceSku) && x.DeviceSku.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                      (!string.IsNullOrEmpty(x.DeviceLocation) && x.DeviceLocation.ToLower().Contains(requestTable.SearchContent.ToLower()))
                  )
              ));
            PaginationHelper<Device> deviceInfo = PaginationHelper<Device>.ToPagedList(devicesList, requestTable.PageNumber, requestTable.PageSize);
            PaginationResponse<DeviceResponse> paginationResponse = new PaginationResponse<DeviceResponse>(deviceInfo.Select(x => new DeviceResponse(x, url, getUserGroup(x.User.GroupId))), deviceInfo.PageInfo);
            return (paginationResponse, "ShowAllSuccess");
        }
        public Group getUserGroup(string groupId)
        {
            return repositoryWrapperMariaDB.Groups.FindByCondition(x => x.GroupId.Equals(groupId)).FirstOrDefault();
        }
        public (List<Device> devices, string message) ShowWithType(string typeDevice)
        {
            List<Device> devicesList = repositoryWrapperMariaDB.Devices
                .FindAll()
                .Include(x => x.User)
                .Where(x => x.DeviceTypeId == typeDevice).ToList();
            if (devicesList is null)
            {
                return (null, "NoData");
            }

            return (devicesList, "ShowAllSuccess");
        }
        public (Device device, string message) Update(string deviceId, UpdateDevice request, string userId, string url = null)
        {
            Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId == deviceId).FirstOrDefault();
            User user = repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault();
            if (device is null)
            {
                return (null, "DeviceNotFound");
            }
            if (user is null)
            {
                return (null, "UserIdNotFound");
            }
            if (!string.IsNullOrEmpty(request.GroupId) && request.GroupId != device.GroupId)
            {
                return (null, "CanNotChangeTheGroup");
            }
            if (!String.IsNullOrEmpty(request.DevicePass))
            {
                string oldPass = GeneralHelper.MD5Hash(request.DeviceOldPass);
                string newPass = GeneralHelper.MD5Hash(request.DevicePass);
                if (device.DevicePass != oldPass)
                {
                    return (null, "WrongPasswordDevice.");
                }
                request.DevicePass = newPass;
            }

            string devicePhoto = device.DevicePhoto;
            if (request.Image != null)
            {
                string path = device.DevicePhoto;
                if (path != null)
                {
                    string mediaUri = GeneralHelper.UrlCombine(url, path);
                    string fileNameDelete = GeneralHelper.GetFileName(mediaUri);
                    var didretoryVideo = GeneralHelper.UrlCombine(GeneralHelper.GetDirectoryFromFile(mediaUri), fileNameDelete);
                    bool checkExists = System.IO.File.Exists(didretoryVideo);
                    if (checkExists)
                    {
                        (bool check, string message) = GeneralHelper.DeleteFile(path);
                        if (!check)
                        {
                            return (null, message);
                        }
                    }
                }
                (string fileName, _) = GeneralHelper.UploadFileV2(request.Image, "device").Result;
                devicePhoto = GeneralHelper.UrlCombine("device", fileName);
            }
            try
            {
                if (request.DeviceExpired != device.DeviceExpired || request.DeviceWarrantyExpiresDate != device.DeviceWarrantyExpiresDate)
                {
                    MqttSingletonService.PingMessage($"{TopicDefine.INFO_DEVICE}/{device.DeviceId}",JsonConvert.SerializeObject(new { deviceExpired = request.DeviceExpired } ),true);
                    deviceLogService.LogUpdate(request, device, userId);
                }
                if (request.DeviceExpired is null)
                {
                    request.DeviceExpired = device.DeviceExpired;
                }
                if (request.DeviceWarrantyExpiresDate is null)
                {
                    request.DeviceWarrantyExpiresDate = device.DeviceWarrantyExpiresDate;
                }
                device = mapper.Map<UpdateDevice, Device>(request, device);
                device.DevicePhoto = devicePhoto;
                device.UserId = userId;
                repositoryWrapperMariaDB.Devices.Update(device);
                repositoryWrapperMariaDB.SaveChanges();
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
            return (device, "UpdateDeviceSuccess");
        }

        public (Device device, string message) UpdateMemory(string deviceId, UpdateMemory request)
        {
            Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId.Equals(deviceId)).FirstOrDefault();
            if (device == null) return (null, "LoginNameNotFound");
            try
            {
                if (request.DeviceMemory != null) device.DeviceMemory = (float)Math.Round(request.DeviceMemory.Value / 1024, 2);
                if (request.DeviceUseMemory != null) device.DeviceUserMemory = (float)Math.Round(request.DeviceUseMemory.Value / 1024, 2);

                repositoryWrapperMariaDB.SaveChanges();
                return (device, "UpdateMemorySuccess");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public (object data, string message) Login(LoginDeviceRequest request)
        {
            Device checkDevice = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.LoginName == request.DeviceIp).FirstOrDefault();
            if (checkDevice == null)
            {
                return (null, "LoginInformationIsIncorrect");
            }
            string pass = (request.DevicePass + checkDevice.DeviceSaft).HashPassword();
            if (!pass.Equals(checkDevice.DevicePass))
            {
                return (null, "WrongPassword");
            }
            (bool sessionLogin, string messageSessionLogin) = SessionLogin(checkDevice);
            if (!sessionLogin)
            {
                return (null, messageSessionLogin);
            }
            if (checkDevice.DeviceStatus != DEVICESTATUS.ACTIVED)
            {
                return (null, "YourAccountIsDisabled");
            }
            if (checkDevice.DeviceExpired < DateTime.Now)
            {
                return (null, "DeviceExpired");
            }
            string tokenDevice = BuildTokenDevice(checkDevice);
            DeviceTokens deviceTokens = new DeviceTokens()
            {
                DeviceID = checkDevice.DeviceId,
                DeviceToken = tokenDevice
            };
            repositoryWrapperMariaDB.DeviceTokens.Add(deviceTokens);
            repositoryWrapperMariaDB.SaveChanges();
            return (new { Device = checkDevice, DeviceToken = deviceTokens.DeviceToken }, "Login success");
        }

        public (bool, string) SessionLogin(Device device)
        {
            try
            {
                List<DeviceTokens> deviceTokens = repositoryWrapperMariaDB.DeviceTokens.FindByCondition(x => x.DeviceID.Equals(device.DeviceId)).OrderBy(x => x.CreatedAt).ToList();
                // lấy phiên đăng nhập từ setting
                (Setting sessionLogin, string messageSessionLogin) = settingService.ShowByWithKey("session_login");
                if (sessionLogin is null)
                {
                    return (false, messageSessionLogin);
                }
                // lấy cờ cho phép change token hay không từ setting
                (Setting ChangeToken, string messageChangeToken) = settingService.ShowByWithKey("change_token");
                if (ChangeToken is null)
                {
                    return (false, messageChangeToken);
                }
                // nếu phiên đăng nhập < 0 thì hết phiên đăng nhập
                if (int.Parse(sessionLogin.SettingValue) <= 0)
                {
                    return (false, "LoginSessionEnd");
                }
                // kiểm tra số lần đăng nhập lớn hơn phiên đăng nhập cho phép ở setting
                if (deviceTokens.Count > int.Parse(sessionLogin.SettingValue))
                {
                    // kiểm tra cho phép change token hay không
                    if (int.Parse(ChangeToken.SettingValue) == 0) // không cho phép change token
                    {
                        return (false, "LoginSessionEnd");
                    }
                    // cho phép change token và remove token cũ nhất
                    repositoryWrapperMariaDB.DeviceTokens.Remove(deviceTokens.FirstOrDefault());
                    repositoryWrapperMariaDB.SaveChanges();
                }
                return (true, "LoginSuccess");
            }
            catch (Exception e)
            {

                return (false, $"{e.StackTrace}-------{e.Message}");
            }

        }

        public (bool result, string message) Logout(string token)
        {

            DeviceTokens deviceTokens = repositoryWrapperMariaDB.DeviceTokens.FindByCondition(x => x.DeviceToken == token).FirstOrDefault();
            if (deviceTokens == null)
            {
                return (false, "Unauthorized");
            }
            repositoryWrapperMariaDB.DeviceTokens.Remove(deviceTokens);
            repositoryWrapperMariaDB.SaveChanges();
            return (true, "LogoutSuccess");
        }
        public (bool result, string message) LogoutCms(string deviceId)
        {
            Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId.Equals(deviceId)).FirstOrDefault();
            if(device is null)
            {
                return (false, "DeviceIdNotFound");
            }
            Console.WriteLine($"********************:{TopicDefine.LOGOUT_DEVICE}/{device.DeviceId}");
            MqttSingletonService.PingMessage($"{TopicDefine.LOGOUT_DEVICE}/{device.DeviceId}", null, false);
            return (true,"LogOutSuccess");
        }

        public (Device device, string message) CheckTokenDevice(string token)
        {
            DeviceTokens deviceToken = repositoryWrapperMariaDB.DeviceTokens.FindByCondition(x => x.DeviceToken == token).FirstOrDefault();
            if (deviceToken == null)
                return (null, "Unauthorized");
            Device device = DecodeTokenGetDevice(token);
            if (device == null || device.DeviceStatus != DEVICESTATUS.ACTIVED)
            {
                return (null, "Unauthorized");
            }
            return (device, "GetDeviceSuccess");
        }

        public string BuildTokenDevice(Device device)
        {
            List<Claim> claims = new List<Claim> { new Claim("deviceIP", device.LoginName), new Claim("deviceId", device.DeviceId) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"].ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"].ToString(),
                configuration["Jwt:Issuer"].ToString(), claims,
                expires: DateTime.UtcNow.AddHours(7).AddYears(int.Parse(configuration["Device:Expires"].ToString())),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public JwtSecurityToken DecodeToken(string token)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }

        public Device DecodeTokenGetDevice(string token)
        {
            string deviceIP = DecodeToken(token).Claims.First(x => x.Type.Equals("deviceIP")).Value;
            return repositoryWrapperMariaDB.Devices.FindByCondition(x => x.LoginName == deviceIP).FirstOrDefault();
        }
        public (int? total, int? totalActive, int? totalDeactive, int? totalWarranty) Dashboard(string url, string userId)
        {
            User user = repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault();
            if (user is null)
            {
                return (null, null, null, null);
            }
            IQueryable<Device> devices = repositoryWrapperMariaDB.Devices
                .FindAll()
                .Include(x => x.User);
            if (!string.IsNullOrEmpty(user.GroupId))
            {
                devices = devices.Where(x => x.GroupId == user.GroupId);
            }

            List<DeviceResponse> data = devices.ToList()
                .Select(x => new DeviceResponse(x, url)).ToList();
            return (data.Count, data.Count(x => x.DeviceStatus == DEVICESTATUS.ACTIVED), data.Count(x => x.DeviceStatus == DEVICESTATUS.DEACTIVATED), data.Count(x => x.DeviceWarrantyExpires == 1));
        }
        public (Device, string) DeleteMemory(string deviceId)
        {
            Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId.Equals(deviceId)).FirstOrDefault();
            if (device is null)
            {
                return (null, "DeviceNotFound");
            }
            device.DeviceMemory = 0;
            repositoryWrapperMariaDB.Devices.Update(device);
            repositoryWrapperMariaDB.SaveChanges();
            
            return (device, "DeleteMemorySuccess");
        }

        public (DeviceResponse, string) ChangeLockDevice(ChangeLockDeviceRequest valueInput, string deviceId, string url)
        {
            Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId.Equals(deviceId)).FirstOrDefault();
            if (device is null)
            {
                return (null, "DeviceNotFound");
            }
            device.DeviceLock = valueInput.DeviceLock;
            repositoryWrapperMariaDB.Devices.Update(device);
            repositoryWrapperMariaDB.SaveChanges();
            return (new DeviceResponse(device, url), "ChangeLockDeviceSuccess");
        }
        public (bool, string) ChangeLockMultiDevice(List<ChangeLockMultiDeviceRequest> valueInput)
        {
            IDbContextTransaction dbContextTransaction = repositoryWrapperMariaDB.BeginTransaction();
            foreach (ChangeLockMultiDeviceRequest data in valueInput)
            {
                Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId.Equals(data.DeviceId)).FirstOrDefault();
                if (device is null)
                {
                    dbContextTransaction.Rollback();
                    return (false, "DeviceNotFound");
                }
                device.DeviceLock = data.DeviceLock;
                repositoryWrapperMariaDB.Devices.Update(device);
                repositoryWrapperMariaDB.SaveChanges();
                Console.WriteLine($"Topic : {TopicDefine.LOCK_DEVICE}/{device.DeviceId}************body : {JsonConvert.SerializeObject(new { status = device.DeviceLock })}");
                MqttSingletonService.PingMessage($"{TopicDefine.LOCK_DEVICE}/{device.DeviceId}", JsonConvert.SerializeObject(new { status = device.DeviceLock }), true);
            }
            dbContextTransaction.Commit();
            return (true, "ChangeLockDeviceSuccess");
        }
        public (DeviceResponse, string) ChangePowerDevice(string deviceId, int value)
        {
            Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId.Equals(deviceId)).FirstOrDefault();
            if (device is null)
            {
                return (null, "DeviceNotFound");
            }
            if (value == 0)
            {
                device.DevicePower = ENUMDEVICEPOWER.OFF;
            }
            else
            {
                device.DevicePower = ENUMDEVICEPOWER.ON;

                MqttSingletonService.PingMessage($"{TopicDefine.SCHEDULE_DEVICE}/{device.DeviceId}",null, false);
                //MqttSingletonService.PingMessage($"{TopicDefine.STATUS_DEVICE}/{device.DeviceId}", JsonConvert.SerializeObject(new { status = (int)device.DeviceStatus }), false);
                //MqttSingletonService.PingMessage($"{TopicDefine.LOCK_DEVICE}/{device.DeviceId}", JsonConvert.SerializeObject(new { status = (int)device.DeviceLock }), false);
            }
            repositoryWrapperMariaDB.Devices.Update(device);
            repositoryWrapperMariaDB.SaveChanges();
            return (new DeviceResponse(device), "ChangePowerDeviceSuccess");
        }
        public (bool data, string message) DeleteMultiMemory(List<string> deviceId)
        {
            IDbContextTransaction dbContextTransaction = repositoryWrapperMariaDB.BeginTransaction();
            foreach (var Id in deviceId)
            {
                Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId.Equals(Id)).FirstOrDefault();
                if (device is null)
                {
                    dbContextTransaction.Rollback();
                    return (false, "DeviceCanNotFound");
                }
                Console.WriteLine($"Topic : {TopicDefine.DELETE_MEMORY_DEVICE}/{device.DeviceId}************body : {JsonConvert.SerializeObject(new { memory = 0 })}");
                MqttSingletonService.PingMessage($"{TopicDefine.DELETE_MEMORY_DEVICE}/{device.DeviceId}", JsonConvert.SerializeObject(new { memory = 0 }), false);
            }
            dbContextTransaction.Commit();
            return (true, "DeleteMemorySuccess");
        }
        public (List<DeviceResponse>, string) RestartMultiDevice(RestartDeviceRequest valueInput)
        {
            List<DeviceResponse> deviceResponses = new List<DeviceResponse>();
            foreach (string deviceId in valueInput.DeviceIds)
            {
                Device device = repositoryWrapperMariaDB.Devices.FindByCondition(x => x.DeviceId.Equals(deviceId)).FirstOrDefault();
                if (device is null)
                {
                    return (null, "DeviceNotFound");
                }
                DeviceResponse data = new DeviceResponse(device);
                data.DeviceRestart = ENUMDEVICERESTART.ON;
                deviceResponses.Add(data);
                Console.WriteLine($"Topic : {TopicDefine.RESTART_DEVICE}/{device.DeviceId}************body : {JsonConvert.SerializeObject(new { status = data.DeviceRestart })}");
                MqttSingletonService.PingMessage($"{TopicDefine.RESTART_DEVICE}/{device.DeviceId}", JsonConvert.SerializeObject(new { status = data.DeviceRestart }), false);
            }
            return (deviceResponses, "RestartMultiDeviceSuccess");
        }
    }
}
