using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Middlewares;
using Project.Modules.App.Entities;
using Project.Modules.App.Requests;
using Project.Modules.App.Services;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Requests;
using Project.Modules.Devices.Services;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportAppController : BaseController
    {
        private readonly IConfiguration _configuration;


        private readonly IDeviceServices _deviceServices;
        private readonly IMediaService _mediaService;
        private readonly IAppSupportService _appSupportService;

        public SupportAppController( IConfiguration configuration, IDeviceServices deviceServices, IMediaService mediaService, IAppSupportService appSupportService)
        {
            _configuration = configuration;


            _deviceServices = deviceServices;
            _mediaService = mediaService;
            _appSupportService = appSupportService;
        }

        /// <summary>
        /// Login device 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDeviceRequest request)
        {
            (object result, string message) = _deviceServices.Login(request);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }

        /// <summary>
        /// Log out device (feat update api get schedule by time)
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            string token = Request.HttpContext.ParsingToken();

            (bool result, string message) = _deviceServices.Logout(token);

            if (!result) return ResponseUnauthorized(message);
            return ResponseOk(result, message);
        }

        /// <summary>
        /// Cập nhật dung lượng thiết bị (Total memory/ used memory)
        /// Thông qua api, MQTT
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("memory")]
        [ValidateDeviceAtribute(IsCheckDeviceExpired = true, IsCheckStatus = true)]
        [MiddlewareFilter(typeof(ValidationDeviceMiddleware))]
        public IActionResult UpdateMemory([FromBody] UpdateMemory request)
        {
            string deviceId = Request.HttpContext.GetDeviceId();

            (Device device, string message) = _deviceServices.UpdateMemory(deviceId, request);
            if (device is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(device, message);
        }


        [HttpGet("download/{mediaId}")]
        [ValidateDeviceAtribute(IsCheckDeviceExpired = true, IsCheckStatus = true)]
        [MiddlewareFilter(typeof(ValidationDeviceMiddleware))]
        public IActionResult DownloadMedia(string mediaId)
        {
            (Media media, string messageMedia) = _mediaService.GetMediaForDevice(mediaId, "");
            if(media is null) return ResponseBadRequest("MediaNotFound");

            var pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", media.MediaUrl);
            bool isFile = System.IO.File.Exists(pathFile);
            if (!isFile) return ResponseBadRequest("FileNotFound");


            var stream = new FileStream(pathFile, FileMode.Open);

            return new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = media.MediaUrl.GetFileNameV2()
            };
        }


        [HttpPost("get-by-time")]
        [ValidateDeviceAtribute(IsCheckDeviceExpired = true, IsCheckStatus = true)]
        [MiddlewareFilter(typeof(ValidationDeviceMiddleware))]
        public IActionResult GetScheduleByTime([FromBody] GetScheduleByTimeRequest request)
        {
            string deviceId = Request.HttpContext.GetDeviceId();

            (List<ScheduleResponse> data,  string message) = _appSupportService.GetScheduleByTime(request, deviceId);
            return ResponseOk(data, message);
        }


        [HttpGet("dateTime")]
        public IActionResult GetDateTime()
        {
            DateTime dateTime = DateTime.UtcNow;


            return ResponseOk(new { DateTime = dateTime , Milliseconds = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds() , Seconds = new DateTimeOffset(dateTime).ToUnixTimeSeconds() }, "GetDateTimeSuccess");
        }





    }
}
