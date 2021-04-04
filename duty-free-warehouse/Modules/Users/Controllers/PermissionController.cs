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
    public class PermissionController : BaseController
    {
        private readonly IServicePermission servicePermission;
        public PermissionController(IServicePermission _servicePermission)
        {
            servicePermission = _servicePermission;
        }


        [HttpPost("add")]
        public IActionResult AddPermission([FromBody] AddPermissionRequest addPermissionRequest)
        {
            (object data, string message) result = servicePermission.CreatePermission(addPermissionRequest);
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }
    }
}