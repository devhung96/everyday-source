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
    public class DepartmentPermissionController : BaseController
    {
        private readonly IServiceDepartmentPermission departmentPermission;
        public DepartmentPermissionController(IServiceDepartmentPermission _departmentPermission)
        {
            departmentPermission = _departmentPermission;
        }

        [HttpPost("add")]
        public IActionResult AddGroupModule([FromBody] AddDepartmentPermissionRequest addGroupModuleRequest)
        {
            (object data, string message) result = departmentPermission.CreatePermissionAndDepartment(addGroupModuleRequest);
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

        [HttpGet("permissions")]
        public IActionResult GetPermissions()
        {
            (object data, string message) result = departmentPermission.GetPermissionWithDepartment();
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

    }
}