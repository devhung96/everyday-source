using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Accounts.Requests;
using Project.Modules.Accounts.Services;
using Project.Modules.Users.Requests;

namespace Project.Modules.Accounts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            (object data, string message) = _accountService.Login(request);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPut("resetPassword")]
        public IActionResult Reset([FromBody] ResetPasswordRequest request)
        {
            (object data, string message) = _accountService.ResetPassword(request);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [Authorize]
        [HttpPost("changePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            string accountId = User.FindFirstValue("AccountId");
            (object data, string message) = _accountService.ChangePassword(request, accountId);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPost("sendMail")]
        public IActionResult SendMail(ForgotRequest request)
        {
            (object data, string message) = _accountService.Fogot(request);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPost("updateForgot/key={key}")]
        public IActionResult UpdatePasswordForgot([FromBody] ForgotPasswordRequest request, string key)
        {
            (object user, string message) = _accountService.FogotUpdatePassword(request, key);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }
        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            string AccountId = User.FindFirstValue("AccountId");
            (object data, string message) = _accountService.Profile(AccountId);

            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data);
        }
    }
}
