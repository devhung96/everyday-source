using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.PermissionOrganizes.Requests;
using Project.Modules.PermissionOrganizes.Services;
using Project.Modules.Users.Entities;

namespace Project.Modules.PermissionOrganizes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionOrganizeController : BaseController
    {
        private readonly IPermissionOrganizeService permissionOrganizeService;
        public PermissionOrganizeController(IPermissionOrganizeService permissionOrganizeService)
        {
            this.permissionOrganizeService = permissionOrganizeService;
        }
        [HttpGet]
        public IActionResult ShowAllPermissionOrganize()
        {
            var result = permissionOrganizeService.ShowAllPermissonOrganize();
            return ResponseOk(result);
        }
        [HttpPost("show-all-user-permission-organize/{organizeId}")]
        public IActionResult ShowAllUser([FromBody] RequestTable request, string organizeId)
        {
            (ResponseTable response, string message) = permissionOrganizeService.ShowAllUserPermissionOrganize(request, organizeId);
            if (response is null)
            {
                return ResponseBadRequest(message);
            }    
            return ResponseOk(response, "");
        }
        [HttpPost("show-permission-organize-by-user")]
        public IActionResult ShowByUser([FromBody] PermissionOrganizeByUserRequest request)
        {
            var result = permissionOrganizeService.ShowPermissionOrganizeOfUser(request);
            return ResponseOk(result);
        }
        [HttpPut("update-permission-organize-by-user")]
        public IActionResult UpdateUser([FromBody] PermissionOrganizeByUserRequest request)
        {
            (User user, string message)  = permissionOrganizeService.UpdatePermissionOrganizeUser(request);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }
        [HttpPost("add-permission-organize-by-user")]
        public IActionResult AddUser([FromBody] PermissionOrganizeByUserRequest request)
        {
            (User user, string message) = permissionOrganizeService.AddPermissionOrganizeUser(request);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);

        }
        [HttpGet("show-all-user-organize/{organizeId}")]
        public IActionResult ShowAll (string organizeId)
        {
            (List<User> users, string message) = permissionOrganizeService.ShowUserOrganize(organizeId);
            if(users is null)
            {
                return ResponseBadRequest(message);

            }
            return ResponseOk(users, message);
        }

    }
}
