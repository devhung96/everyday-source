using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Requests;
using Project.Modules.Devices.Services;
using Project.Modules.Users.Middlewares;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json.Linq;
using Project.App.Helpers;
using Microsoft.Extensions.Configuration;
using Project.App.Mqtt;
using Project.App.Middlewares;

namespace Project.Modules.Devices.Controllers
{
    [Route("api/device")]
    [ApiController]
    [Authorize]
    //[MiddlewareFilter(typeof(CheckUserExistsMiddlewareAttribute))]
    public class DeviceController : BaseController
    {
        private readonly IDeviceServices _deviceServices;
        private readonly IConfiguration configuration;
        private readonly int isHttps = 0;
        public DeviceController(IDeviceServices deviceServices, IConfiguration _configuration)
        {
            _deviceServices = deviceServices;
            configuration = _configuration;
            isHttps = _configuration["IsHttps"].toInt();
        }
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;
            (int? total, int? totalActive, int? totalDeactive, int? totalWarranty) = _deviceServices.Dashboard(urlRequest, userId);
            return ResponseOk(new { total, totalActive, totalDeactive, totalWarranty }, "DashboardSuccess");
        }
        [HttpPost]
        public IActionResult Store([FromForm] StoreDevice request)
        {
            string userId = User.FindFirst("user_id").Value;
            string imageName = "";
            if (request.Image != null)
            {
                (string fileName, _) =  GeneralHelper.UploadFileV2(request.Image, "device").Result;
                 imageName = GeneralHelper.UrlCombine("device", fileName);
            }
            (Device device, string message) = _deviceServices.Store(request, userId, imageName);
            if(device is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(device, message);
        }

        [HttpGet("{deviceID}")]
        public IActionResult FindID(string deviceID)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (Device device, string message) = _deviceServices.FindID(deviceID);
            if(device is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(new DeviceResponse(device, urlRequest), message);
        }

        [HttpGet("active/{deviceId}")]
        public IActionResult Active(string deviceId)
        {
            (Device device, string message) = _deviceServices.Active(deviceId);
            if(device is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(device, message);
        }
        [HttpPut("changeActiveMultiDevice")]
        public IActionResult ChangeActiveMultiDevice(List<ChangeActiveMultiRequest> valueInput)
        {
            (List<Device> devices, string message) = _deviceServices.ChangeActiveMultiDevice(valueInput);
            if (devices is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(devices, message);
        }

        [HttpDelete("{deviceId}")]
        public IActionResult Delete(string deviceId)
        {
            string userId = User.FindFirst("user_id").Value;
            (Device result, string message) = _deviceServices.Delete(deviceId, userId);
            if(result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(null,message);
        }
        [HttpGet]
        public IActionResult Show([FromQuery] FilterDeviceRequest requestTable)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string groupId = User.FindFirst("group_id").Value;
            (PaginationResponse<DeviceResponse> listDevice, string message) = _deviceServices.ShowAllPagination(requestTable, groupId, urlRequest);
            return ResponseOk(listDevice, message);
        }
        [MiddlewareFilter(typeof(CheckUserSuperAdminMiddleware))]
        [HttpGet("all")]
        public IActionResult ShowAllSuperAdmin([FromQuery] FilterDeviceRequest requestTable)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (PaginationResponse<DeviceResponse> listDevice, string message) = _deviceServices.ShowAllPagination(requestTable, urlRequest);
            return ResponseOk(listDevice, message);
        }
        [HttpGet("showByGroup")]
        public IActionResult ShowByGroup([FromQuery] FilterDeviceRequest requestTable)
        {
            string groupId = User.FindFirst("group_id")?.Value.ToString();
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (PaginationResponse<DeviceResponse> listDevice, string message) = _deviceServices.ShowAllByPagination(requestTable, groupId , urlRequest);
            return ResponseOk(listDevice, message);
        }
        [HttpPut("{deviceId}")]
        public IActionResult Update(string deviceId, [FromForm] UpdateDevice storeDevice)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;
            (Device device, string message) = _deviceServices.Update(deviceId, storeDevice, userId, urlRequest);
            if(device is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(device, message);
        }
        [HttpPut("deleteMemory/{deviceId}")]
        public IActionResult DeleteMemory(string deviceId)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (Device device, string message) = _deviceServices.DeleteMemory(deviceId);
            if(device is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(new DeviceResponse(device, urlRequest), message);
        }
        [HttpPut("deleteMultiMemory")]
        public IActionResult DeleteMultiMemory([FromBody] DeleteMultiMemoryRequest valueInput)
        {
            (bool device, string message) = _deviceServices.DeleteMultiMemory(valueInput.deviceIds);
            if (device == false)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(null, message);
        }
        [HttpPut("changeLockDevice/{deviceId}")]
        public IActionResult ChangeLockDevice([FromBody] ChangeLockDeviceRequest valueInput, string deviceId)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (DeviceResponse data, string message) = _deviceServices.ChangeLockDevice(valueInput, deviceId, urlRequest);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPut("changeLockMultiDevice")]
        public IActionResult ChangeLockMultiDevice([FromBody] List<ChangeLockMultiDeviceRequest> valueInput)
        {
            (bool data, string message) = _deviceServices.ChangeLockMultiDevice(valueInput);
            if (data == false)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpDelete("multi")]
        public IActionResult DeleteMulti([FromBody] DeleteMultiDeviceRequest valueInput)
        {
            string userId = User.FindFirst("user_id").Value;
            (object result, string message) = _deviceServices.DeleteMulti(valueInput, userId);
            if(result == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(null, message);
        }
        [HttpPut("restartMultiDevice")]
        public IActionResult RestartMultiDevice(RestartDeviceRequest valueInput)
        {
            (List<DeviceResponse> data, string message) = _deviceServices.RestartMultiDevice(valueInput);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPut("ChangePowerDevice")]
        public IActionResult ChangePowerDevice(string deviceId, int value)
        {
            (DeviceResponse data, string message) = _deviceServices.ChangePowerDevice(deviceId, value);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpGet("LogOutCMS/{deviceId}")]
        public IActionResult LogOutCMS(string deviceId)
        {
            (bool data, string message) = _deviceServices.LogoutCms(deviceId);
            if(data == false)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(null, message);
        }
    }
}