using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Modules.Settings.Validations;
using Project.Modules.Settings.Entitites;
using Project.Modules.Settings.Services;
using Project.App.Controllers;
using Project.Modules.Settings.Requests;
using Newtonsoft.Json;
using Project.App.Helpers;

namespace Project.Modules.Settings.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[MiddlewareFilter(typeof(CheckUserExistsMiddlewareAttribute))]
    public class SettingController : BaseController
    {
        private readonly ISettingService _settingService;
        public SettingController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        /// <summary>Create setting mới </summary>
        /// <remarks>
        /// Sample value of message
        /// 
        ///     POST /Todo
        ///     {
        ///        "SettingKey": "Key1",
        ///        "SettingValue": {key:"Key1"},
        ///        "SettingType" : 2
        ///     }
        ///     SettingType : {
        ///         String : 1,
        ///         Object : 2,
        ///         Array : 3
        ///     }
        ///     
        /// </remarks>
        [HttpPost]
        public IActionResult Store([FromBody] StoreValidation input)
        {
            string value = "";
            if (input.SettingType == EnumSettingType.String)
            {
                value = input.SettingValue.ToString();
            }else
            {
                value = JsonConvert.SerializeObject(input.SettingValue);
            }
            Setting newSetting = new Setting()
            {
                SettingKey = input.SettingKey,
                SettingValue = value,
                SettingType = (int)input.SettingType
            };
            (Setting setting, string message) = _settingService.Store(newSetting);
            return ResponseOk(setting, message);
        }
        /// <summary>Update setting </summary>
        /// <remarks>
        /// Sample value of message
        /// 
        ///     Put /Todo
        ///     {
        ///        "SettingKey" : "SettingKey"
        ///        "SettingValue": {key:"Key1"},
        ///        "SettingType" : 2
        ///     }
        ///     SettingType : {
        ///         String : 1,
        ///         Object : 2,
        ///         Array : 3
        ///     }
        /// </remarks>
        [HttpPut("{key}")]
        public IActionResult Update([FromBody] UpdateSettingRequest input, string key)
        {
            (Setting setting, string message) = _settingService.Update(input, key);
            if (setting == null) return ResponseBadRequest(message);
            return ResponseOk(setting, message);
        }
        /// <summary>show detail setting </summary>
        [HttpGet("show-by-with-key/{key}")]
        public IActionResult ShowByWithKey(string key)
        {
            (Setting setting, string message) = _settingService.ShowByWithKey(key);
            if (setting == null) return ResponseBadRequest(message);
            SettingReponse settingReponse = new SettingReponse(setting);
            return ResponseOk(settingReponse, message);
        }
        /// <summary>Get All Setting </summary>
        [HttpGet]
        public IActionResult ShowAll([FromQuery] PaginationRequest paginationRequest)
        {
            (PaginationResponse<SettingReponse> paginationResponse, string message) = _settingService.ShowAll(paginationRequest);
            return ResponseOk(paginationResponse, message);
        }
        /// <summary>Delete Setting </summary>
        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            (Setting setting, string message) = _settingService.ShowByWithKey(key);
            if (setting == null) return ResponseBadRequest(message);
            (_ , string messageDelete) = _settingService.Delete(setting);
            return ResponseOk(null, "messageDelete");
        }

    }
}