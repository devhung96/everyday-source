using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Organizes.Entities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/ForgotPassword")]
    [ApiController]
    public class ForgotPasswordUserInEventController : BaseController
    {
        private readonly IForgotPasswordService forgotPasswordService;
        public ForgotPasswordUserInEventController(IForgotPasswordService forgotPasswordService)
        {
            this.forgotPasswordService = forgotPasswordService;
        }
        
        [HttpPost("SendMail")]
        public IActionResult SendKey([FromBody] RequestForgot request)
        {
            (User user, string message) = forgotPasswordService.CheckEmail(request);
            if (user is null)
                return ResponseBadRequest(message);

            return ResponseOk(request.ShareholderCode, "");
        }

        [HttpGet("CheckKey/key={key}")]
        public IActionResult CheckKey(string key)
        {
            var (email, message) = forgotPasswordService.CheckKey(key);
            if (email is null)
                return ResponseBadRequest(message);

            return ResponseOk(email, message);
        }

        [HttpPut("UpdatePasswordNew/key={key}")]
        public IActionResult UpdatePasswordNew([FromBody] ForgotPasswordRequest request, string key)
        {
            (EventUser user, string message) = forgotPasswordService.UpdatePassword(request, key);

            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, "Cập nhật mật khẩu thành công.");
        }

    }
}