using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceLogsController : BaseController
    {
        private readonly IDeviceLogService deviceLogService;

        public DeviceLogsController(IDeviceLogService _deviceLogService)
        {
            deviceLogService = _deviceLogService;

        }
        [HttpGet]
        public IActionResult ShowAll([FromQuery] PaginationRequest paginationRequest)
        {
            (PaginationResponse<ResponseDeviceLog> devices, string message) = deviceLogService.ShowAll(paginationRequest);
            return ResponseOk(devices, message);
        }
        [HttpGet("showAllByDevice/{deviceId}")]
        public IActionResult ShowAll([FromQuery] PaginationRequest paginationRequest, string deviceId)
        {
            (PaginationResponse<ResponseDeviceLog> devices, string message) = deviceLogService.ShowAllByDevice(paginationRequest, deviceId);
            return ResponseOk(devices, message);
        }
    }
}
