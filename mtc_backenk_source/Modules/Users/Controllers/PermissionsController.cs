using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : BaseController
    {
        private readonly IPermissionService permissionService;
        public PermissionsController(IPermissionService permissionService)
        {
            this.permissionService = permissionService;
        }

        /// <summary>
        ///  USER = 0 ||
        ///  SUPERADMIN = 1
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpGet("showByLevel")]
        public IActionResult GetPermissionByLevel([FromQuery]int level)
        {
            (object data, string message) = permissionService.ShowAllPermission((UserLevelEnum)level);

            return ResponseOk(data, message);
        }
        [Authorize]
        [HttpGet]
        public IActionResult ShowPermisionByToken()
        {
            int level = int.Parse(User.FindFirst("is_level").Value.ToString());
            (object data, string message) = permissionService.ShowAllPermission((UserLevelEnum)level);

            return ResponseOk(data, message);
        }
        [HttpPost]
        public IActionResult Create ([FromBody] CreatePermissionRequest newPermission)
        {
            (object data, string message) = permissionService.CreatePermission(newPermission);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
    }
}
