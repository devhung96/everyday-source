using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Middleware;
using Project.Modules.Permissions.Entities;
using Project.Modules.Permissions.Requests;
using Project.Modules.Permissions.Services;

namespace Project.Modules.Permissions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [MiddlewareFilter(typeof(CheckTokenMiddleware))]
  //  [Authorize(Roles = "ADMINISTRATOR_SYSTEM")]

    public class PermissionController : BaseController
    {
        private readonly IPermissionServices permissionServices;
        private readonly IConfiguration configuration;
        public PermissionController(IPermissionServices _permissionServices, IConfiguration _configuration)
        {
            permissionServices = _permissionServices;
            configuration = _configuration;
        }
        //[HttpPost("AddPermission")]
        //public IActionResult AddPermission([FromBody] AddPermission data)
        //{
        //    Permission permission = permissionServices.Addpermission(data);
        //    return ResponseOk(new { data }, "Thêm thành công");
        //}
        //[HttpDelete("DeletePermission/{PermissionCode}")]
        //public IActionResult DeletePermission(string PermissionCode)
        //{
        //    Permission permission = permissionServices.Deletepermission(PermissionCode);
        //    if( permission is null)
        //    {
        //        return ResponseBadRequest("PermissionCode is not exist.");
        //    }    
        //    return ResponseOk(new { PermissionCode }, "Permission deleted success.");
        //}
        [HttpGet]
        public IActionResult ShowlistData()
        {
            List<Entities.PermissionOrganize> lst = permissionServices.List();
            return ResponseOk(new { lst });
        }
    }
}
