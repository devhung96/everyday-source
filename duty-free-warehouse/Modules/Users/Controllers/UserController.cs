using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Middleware;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Request;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/user")]
    [ApiController]
   
    public class UserController : BaseController
    {
        private readonly IServiceUser serviceUser;
        public UserController(IServiceUser _serviceUser)
        {
            serviceUser = _serviceUser;
        }
        [Authorize]
        [MiddlewareFilter(typeof(CheckTokenMiddleware))]
        [HttpPost("create")]
        public IActionResult CreateNewUser([FromBody]RequestCreateNewUser requestCreateNew)
        {
            var result = serviceUser.CreateNewUser(requestCreateNew);
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }
        [Authorize]
        [MiddlewareFilter(typeof(CheckTokenMiddleware))]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var result = serviceUser.GetAllUser();
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]RequestLogin requestCreateNew)
        {
            var result = serviceUser.Login(requestCreateNew);
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

        [Authorize]
        [MiddlewareFilter(typeof(CheckTokenMiddleware))]
        [HttpPost("edit")]
        public IActionResult UpdateUser([FromQuery] int userId,[FromBody]UpdateUserRequest requestCreateNew)
        {
            var result = serviceUser.UpdateUser(userId,requestCreateNew);
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

        [Authorize]
        [MiddlewareFilter(typeof(CheckTokenMiddleware))]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody]ChangePasswordRequest changepass)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = serviceUser.ChangePassword(accessToken,changepass);
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }


        [Authorize]
        [MiddlewareFilter(typeof(CheckTokenMiddleware))]
        [HttpPost("delete")]
        public IActionResult DeleteUser([FromQuery] int userId)
        {
            var result = serviceUser.DeleteUser(userId);
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }
        [HttpPost("sendMail")]
        public IActionResult SendMail([FromBody] SendMailForgot request)
        {
            (User user, string message) = serviceUser.CheckEmail(request.Email);
            if(user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }
        [HttpPut("updatePasswordNew/key={key}")]
        public IActionResult UpdatePassword([FromBody] ForgotPasswordRequest request, string key)
        {
            (User user, string message)= serviceUser.UpdatePassword(request, key);
            if(user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk("Thành công!!!", message);
        }
    }



}