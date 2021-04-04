using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Logs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Logs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : BaseController
    {
        private readonly ILogService LogService;

        public LogsController(ILogService logService)
        {
            LogService = logService;
        }

        /// <summary>
        /// ### Effect -- Get tất cả log (Detections)
        /// ### Artist -- An
        /// ### Des -- Frontend
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllLog([FromQuery] PaginationRequest request)
        {
            return ResponseOk(LogService.GetAllLog(request), "Success");
        }
    }
}
