using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Logs.Entities;
using Project.Modules.Logs.Requests;
using Project.Modules.Logs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Logs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppLogController : BaseController
    {
        private readonly IAppLogService appLogService;
        public AppLogController(IAppLogService appLogService)
        {
            this.appLogService = appLogService;
        }
        [HttpPost]
        public IActionResult AddNew([FromBody] AppLogRequest request)
        {
            AppLog appLog = appLogService.Create(request);
            return ResponseOk(appLog);
        }
        [HttpGet("{key}")]
        public IActionResult Detail (string key)
        {
            AppLog appLog = appLogService.Detail(key);
            return ResponseOk(appLog);
        }
        [HttpGet()]
        public IActionResult ShowAll ()
        {
            List<AppLog> appLogs = appLogService.ShowAll();
            return ResponseOk(appLogs);
        }
    }
}
