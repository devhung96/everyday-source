using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.DeviceTypes.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeviceTypes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceTypesController : BaseController
    {
        private readonly IDeviceTypeService deviceTypeService;
        public DeviceTypesController(IDeviceTypeService deviceTypeService)
        {
            this.deviceTypeService = deviceTypeService;
        }
        [HttpGet("{deviceTypeId}")]
        public IActionResult Detail(string deviceTypeId)
        {
            (DeviceType deviceType, string message)  = deviceTypeService.Detail(deviceTypeId);
            if(deviceType is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(deviceType, message);
        }
        [HttpGet("showAll")]
        public IActionResult ShowAll()
        {
            return ResponseOk(deviceTypeService.ShowAll());
        }
        [HttpGet]
        public IActionResult ShowTable([FromQuery] PaginationRequest request)
        {
            return ResponseOk(deviceTypeService.ShowTable(request));
        }
    }
}
