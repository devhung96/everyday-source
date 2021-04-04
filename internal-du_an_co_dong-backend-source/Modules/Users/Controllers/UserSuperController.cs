using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Requests;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSuperController : BaseController
    {
        private readonly IUseSuperService useSuperService;
        public UserSuperController(IUseSuperService useSuperService)
        {
            this.useSuperService = useSuperService;
        }
        [Authorize]
        [HttpGet("get-profile")]
        public IActionResult GetProfile()
        {
            string userSuperId = User.FindFirstValue("UserId").ToString();
            var (superAdmin, message) = useSuperService.GetUserSuper(userSuperId);
            return ResponseOk(superAdmin, message);
        }
        [HttpPost("add-user-super-admin")]
        public IActionResult StoreUserSuper([FromBody] AddUserSuperRequest request)
        {
            (UserSuper superAdmin, string message)  = useSuperService.AddUserSuper(request);
            if(superAdmin is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(superAdmin);
        }
        [HttpPut("update-user-super-admin/{userSuperId}")]
        public IActionResult EditUserSuper([FromBody] UpdateUserSuperRequest request, string userSuperId)
        {
            (UserSuper superAdmin, string message) = useSuperService.EditUserSuper(userSuperId,request);
            if (superAdmin is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(superAdmin);
        }
        [HttpDelete("delete-user-super-admin/{userSuperId}")]
        public IActionResult DeleteUserSuper( string userSuperId)
        {
            (UserSuper superAdmin, string message) = useSuperService.DeleteUserSuper(userSuperId);
            if (superAdmin is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(superAdmin,message);
        }
        [HttpPost("show-list-super-admin")]
        public IActionResult GetListSuperAdmin ([FromBody] RequestTable request)
        {
            ResponseTable data = useSuperService.ListSuperAdmin(request);
            return ResponseOk(data);
        }
    }
}
