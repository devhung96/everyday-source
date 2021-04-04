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
    public class RolesController : BaseController
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService _permissionService)
        {
            this._roleService = _permissionService;
        }
        //[MiddlewareFilter(typeof(CheckPermissonUserMiddlewareAttribute))]
        //[Authorize(Roles = "USER_PERMISSON")]

        /// <summary>
        ///  USER = 0 ||
        ///  SUPERADMIN = 1 ||
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpGet("showByLevel")]
        public IActionResult ShowAllByLevel([FromQuery] int level)
        {
            (object data, string message) = _roleService.ShowAllByLevel((UserLevelEnum)level);
            return ResponseOk(data, message);
        } 
        [Authorize]
        [HttpGet("showRoles")]
        public IActionResult ShowByToken()
        {
            int level = int.Parse(User.FindFirst("is_level").Value.ToString());
            string groupId = User.FindFirst("group_id").Value.ToString();
            (object data, string message) = _roleService.ShowAllRole((UserLevelEnum)level, groupId);
            return ResponseOk(data, message);
        }

        [HttpGet("inGroup/{groupId}")]
        public IActionResult inGroup(string groupId)
        {
            (object data, string message) = _roleService.ShowAllRole(UserLevelEnum.USER, groupId);
            return ResponseOk(data, message);
        }
        [HttpGet("{idRole}")]
        public IActionResult ShowDetail(string idRole )
        {
            (object data, string message) = _roleService.Detail(idRole);
            return ResponseOk(data, message);
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateRole([FromBody] CreateRoleRequest roleRequest)
        {
            string groupId = User.FindFirst("group_id").Value;
            int level = int.Parse(User.FindFirst("is_level").Value);    
            (object data, string message) = _roleService.Create(roleRequest, groupId,level);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPut("{idRole}")]
        public IActionResult Update(string idRole,[FromBody] UpdateRoleRequest request)
        {
            (object data, string message) = _roleService.Update(request,idRole);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpDelete("{idRole}")]
        public IActionResult Update(string idRole)
        {
            (object data, string message) = _roleService.Delete(idRole);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
    }

}
