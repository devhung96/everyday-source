using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpPost("store")]
        public async Task<IActionResult> Store([FromForm]AddUserRequest request)
        {
            (User user, string message) =await  userService.Store(request);
           if(user is null)
            {
                return ResponseBadRequest( message);
            }
            return ResponseOk (user, message);
        }   
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> Update([FromForm]UpdateUserRequest request, string userId)
        {
            (User user, string message) =await  userService.Update(request, userId);
           if(user is null)
            {
                return ResponseBadRequest( message);
            }
            return ResponseOk (user, message);
        }
        [HttpGet("detail/{userId}")]
        public IActionResult Detail(string userId)
        {
            (User user, string message) = userService.Detail(userId);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }
        [HttpDelete("delete/{userId}")]
        public IActionResult Delete(string userId)
        {
            (User user, string message) = userService.Delete(userId);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }
        [HttpPost("showAll")]
        public IActionResult ShowAll([FromBody] RequestTable request)
        {
            (ResponseTable data, string message) = userService.ShowAll(request);
            return ResponseOk(data, message);
        }
        [HttpPut("updatePermission")]
        public IActionResult UpdatePermission([FromBody] UpdatePermissionRequest request)
        {
            (User user, string message)  = userService.UpdatePermission(request);
            if(user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }
    }
}
