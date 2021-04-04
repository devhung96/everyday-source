using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Middleware;
using Project.Modules.Users.Request;
using Project.Modules.Users.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MiddlewareFilter(typeof(CheckTokenMiddleware))]
    public class ModuleController : BaseController
    {
        private readonly IModuleService moduleService;
        public ModuleController(IModuleService _moduleService)
        {
            moduleService = _moduleService;
        }


        [HttpPost("add")]
        public IActionResult AddModule([FromBody] AddModuleRequest addModuleRequest)
        {
            (object data, string message) result = moduleService.CreateModule(addModuleRequest);
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

        [HttpGet("permissions")]
        public IActionResult GetPermissions()
        {
            (object data, string message) result = moduleService.GetPermissionModule();
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }
    }
}