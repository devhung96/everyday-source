using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class UserPermissionController : BaseController
    {
        private readonly IServiceUserPermission servicePermissionUser;
        public UserPermissionController(IServiceUserPermission _servicePermissionUser)
        {
            servicePermissionUser = _servicePermissionUser;
        }

        [HttpPost("update")]
        public IActionResult UpdatePermissionUser([FromBody] UpdatePermissionUser addGroupModuleRequest)
        {
            (object data, string message) result = servicePermissionUser.UpdateUserPermission(addGroupModuleRequest);
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

        [HttpGet]
        public IActionResult GetPermissionUser([FromQuery] int userId)
        {
            (object data, string message) result = servicePermissionUser.GetPermissionUser(userId);
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

    }
}