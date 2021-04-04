using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Requests;
using Project.Modules.Devices.Services;

namespace Project.Modules.Devices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : BaseController
    {
        private readonly IDeviceService deviceSevice;
        public DevicesController(IDeviceService deviceSevice)
        {
            this.deviceSevice = deviceSevice;
        }
        [HttpGet("{deviceId}")]
        public IActionResult Detail(string deviceId)
        {
            (Device result, string message) = deviceSevice.Detail(deviceId);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        [HttpGet]
        public IActionResult ShowTable([FromQuery] PaginationRequest request)
        {
            PaginationResponse<Device> data = deviceSevice.ShowTable(request);
            return ResponseOk(data, "ShowListSuccess");
        }
        [HttpPost]
        public IActionResult Store([FromBody] AddDeviceRequest request)
        {
           ( Device device, string message) = deviceSevice.Store(request);
            if(device is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(device, "AddSuccess");
        }
        [HttpPut("{deviceId}")]
        public IActionResult Update([FromBody] UpdateDeviceRequest request,string deviceId)
        {
            (Device device, string message) = deviceSevice.Update(request,deviceId);
            if (device is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(device, "UpdateSuccess");
        }
        [HttpDelete("{deviceId}")]
        public IActionResult Remove(string deviceId)
        {
            (Device result, string message) = deviceSevice.Delete(deviceId);

            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
    }

}
