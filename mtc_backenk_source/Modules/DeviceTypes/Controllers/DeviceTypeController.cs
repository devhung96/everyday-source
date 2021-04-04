using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Helpers;
//using Project.Modules.Devices.Entities;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.DeviceTypes.Requests;
using Project.Modules.DeviceTypes.Services;
//using Project.Modules.Medias.Entites;
//using Project.Modules.Users.Middlewares;

namespace Project.Modules.DeviceTypes.Controllers
{
    [Route("api/devicetype")]
    [ApiController]
    [Authorize]
    //[MiddlewareFilter(typeof(CheckUserExistsMiddlewareAttribute))]
    public class DeviceTypeController : BaseController
    {
        private readonly IDeviceTypeServices _deviceTypeServices;
        private readonly IConfiguration _configuration;
        private readonly int isHttps = 0;
        public DeviceTypeController(IDeviceTypeServices deviceTypeServices, IConfiguration configuration)
        {
            _deviceTypeServices = deviceTypeServices;
            _configuration = configuration;
            isHttps = _configuration["IsHttps"].toInt();
        }
        
        [HttpPost]
        public async Task<IActionResult> Store([FromForm] CreateDeviceType request)
        {
            DeviceType newDeviceType = new DeviceType()
            {
                DeviceTypeName = request.typeName,
                DeviceTypeComment = request.typeComment
            };
            if (request.typeIcon != null)
            {
                (string fileName,_) = await GeneralHelper.UploadFileV2(request.typeIcon, "deviceTypes");
                newDeviceType.DeviceTypeIcon = GeneralHelper.UrlCombine("deviceTypes", fileName);
            }
            (DeviceType result, string message) = _deviceTypeServices.Store(newDeviceType);

            return ResponseOk(result, message);
        }
        [HttpDelete("{deviceTypeId}")]
        public IActionResult DeleteDeviceType(string deviceTypeId)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (bool result, string message) = _deviceTypeServices.Delete(deviceTypeId, urlRequest);
            if(!result)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(message);
        }
        [HttpGet("{deviceTypeId}")]
        public IActionResult FindID(string deviceTypeId)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (DeviceType result, string message) = _deviceTypeServices.FindID(deviceTypeId, urlRequest);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result,message);
        }
        [HttpPut("{deviceTypeId}")]
        public async Task<IActionResult> Update([FromForm] UpdateDeviceTypeRequest request, string deviceTypeId)
        {
            (DeviceType result, string message) = _deviceTypeServices.Update(deviceTypeId, request);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }


        [HttpGet]
        public IActionResult Show([FromQuery] PaginationRequest requestTable)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (IQueryable<DeviceType> result, string message) = _deviceTypeServices.ShowAll(urlRequest);
            #region Paganation
            result = result.Where(x =>
                string.IsNullOrEmpty(requestTable.SearchContent) ||
                (!string.IsNullOrEmpty(requestTable.SearchContent) &&
                    (
                        x.DeviceTypeName.ToLower().Contains(requestTable.SearchContent.ToLower()) ||
                        x.DeviceTypeComment.ToLower().Contains(requestTable.SearchContent.ToLower())
                    )
                ));
            PaginationHelper<DeviceType> deviceTypeInfo = PaginationHelper<DeviceType>.ToPagedList(result, requestTable.PageNumber, requestTable.PageSize);
            PaginationResponse<DeviceType> paginationResponse = new PaginationResponse<DeviceType>(deviceTypeInfo, deviceTypeInfo.PageInfo);
            #endregion
            return ResponseOk(paginationResponse,"ShowAllSuccess");
        }
    }
}