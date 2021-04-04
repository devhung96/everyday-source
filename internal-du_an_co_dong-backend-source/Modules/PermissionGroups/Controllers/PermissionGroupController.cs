using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.App.Middleware;
using Project.Modules.PermissionGroups.Entities;
using Project.Modules.PermissionGroups.Requests;
using Project.Modules.PermissionGroups.Services;

namespace Project.Modules.PermissionGroups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[MiddlewareFilter(typeof(CheckTokenMiddleware))]
    //[Authorize(Roles = "ADMINISTRATOR_SYSTEM")]
    public class PermissionGroupController : BaseController
    {
        private readonly IPermissionGroupServices permissionGroupServices;
  
        public PermissionGroupController( IPermissionGroupServices _permissionGroupServices)
        {
            this.permissionGroupServices = _permissionGroupServices;
        }
        [HttpPost("Add")]
        public IActionResult AddPermissionGroup([FromBody] AddRequest data)
        {
            (List<PermissionGroup> permissionGroups,string message) = permissionGroupServices.Add(data);
            if (permissionGroups is null)
                return ResponseBadRequest(message);
            return ResponseOk(permissionGroups, "AddPermissionGroupSuccess");
        }
        [HttpPut("Delete/{PermissionGroupId}")]
        public IActionResult DeletePermissionGroup(int PermissionGroupId)
        {
            PermissionGroup permissionGroup = permissionGroupServices.Delete(PermissionGroupId);
            if (permissionGroup is null)
            {
                return ResponseBadRequest("PermissionGroupNotExist");
            }
            return ResponseOk(permissionGroup, "DeletePermissionGroupSuccess");
        }



        [HttpGet("ListByGroup/{GroupId}")]
        public IActionResult ListPermission(int GroupId)
        {
            List<PermissionGroup> lst = permissionGroupServices.GetGroups(GroupId);
            return ResponseOk(lst);
        }

    }
}