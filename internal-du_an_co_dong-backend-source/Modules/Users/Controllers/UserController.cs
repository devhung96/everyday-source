using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.App.Middleware;
using Project.App.Requests;
using Project.Modules.Organizes.Entities;
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
        private readonly IUserSupportService userSupportService;
        public UserController(IUserService userService, IUserSupportService userSupportService)
        {
            this.userService = userService;
            this.userSupportService = userSupportService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetProfile()
        {
            string userId = User.FindFirstValue("UserId").ToString();
            (User user, string message) = userService.GetUser(userId);
            if (user is null)
                return ResponseBadRequest(message);
            return ResponseOk(user);
        }

        [Authorize(Roles = "CLIENT")]
        [HttpPut("changePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest input)
        {
            var userId = User.FindFirstValue("UserID").ToString();
            var eventId = User.FindFirstValue("EventId").ToString();
            (EventUser user, string message) = userService.ChangePassword(input, userId, eventId);
            if (user is null)
                return ResponseBadRequest(message);
            return ResponseOk(user, message);
        }

        [Authorize]
        [HttpPut("changePasswordCMS")]
        public IActionResult ChangePasswordCMS([FromBody] ChangePasswordRequest request)
        {
            string userId = User.FindFirstValue("UserID");
            string organizeId = User.FindFirstValue("OrganizeId");
            if (String.IsNullOrEmpty(organizeId))
            {

                return ResponseUnauthorized("TokenFaild");
            }
            (object user, string message) = userService.ChangePasswordCMS(request, userId);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }

        //[MiddlewareFilter(typeof(CheckTokenMiddleware))]
        //[Authorize(Roles ="Assss")]
        [Authorize]
        [HttpPut("updateProfile")]
        public IActionResult UpdateProfile([FromBody] UpdateUserRequest updateUser)
        {
            string userId = User.FindFirstValue("UserId").ToString();
            (object user, string messageEdit) = userService.EditUser(updateUser, userId);
            if (user is null)
                return ResponseBadRequest(messageEdit);
            return ResponseOk(user);
        }


        [HttpPost("login-superadmin")]
        public IActionResult LoginSuperAdmin([FromBody] LoginCMSRequest request)
        {
            (object data, string message) = userSupportService.LoginSuperAdmin(request.Recaptcha, request.UserName, request.UserPass);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }

        [HttpPost("login-cms")]
        public IActionResult LoginCMS([FromBody] LoginCMSRequest request)
        {
            (object data, string message) = userSupportService.LoginCMS(request.Recaptcha, request.UserName, request.UserPass, request.StockCode);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }


        [Authorize]
        [HttpGet("refresh-token")]
        public IActionResult RefreshToken()
        {

            var prepareUserId = User.FindFirst("UserId");
            var preparePermissonDefault = User.FindFirst("PermissionDefault");
            if (prepareUserId is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess", "AccountDoesNotHaveAccess");
            }

            if (preparePermissonDefault is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess", "AccountDoesNotHaveAccess");
            }
            string userId = prepareUserId.Value.ToString();
            string permissionDefault = preparePermissonDefault.Value;

            (string token, string message) = userSupportService.RefreshToken(userId, permissionDefault);
            if (String.IsNullOrEmpty(token)) return ResponseBadRequest(message);
            return ResponseOk(new { token = token }, message);
        }


        [HttpPost("login-client")]
        public IActionResult LoginClient([FromBody] LoginClientRequest request)
        {
            (object data, string message, string code) = userService.LoginClient(request.Recaptcha, request.ShareholderCode, request.UserPass, request.EventId);
            if (data is null) return ResponseBadRequest(message, code);
            return ResponseOk(data, message, code);
        }



        [Authorize(Roles = "AUTHENCATION-CLIENT-STEP1")]
        [HttpPost("authencation-account")]
        [RequestSizeLimit(long.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]

        public async Task<IActionResult> AuthencationAccountAsync([FromForm] AuthencationAccountRequest request)
        {
            Console.WriteLine(JsonConvert.SerializeObject(request));
            Console.WriteLine("1" + request.UserPhotoUrls[0].FileName);
            Console.WriteLine("2" + request.UserIdentityCardUrl.FileName);
            var userId = User.FindFirst("UserId").Value.ToString();
            var eventId = User.FindFirst("EventId").Value.ToString();
            (object data, string message, string code) = await userService.AuthenticationAccountAsync(userId, eventId);
            if (data is null) return ResponseBadRequest(message, code);
            return ResponseOk(data, message, code);
        }


        [Authorize]
        [HttpGet("getTokenOpenVidu")]
        public IActionResult CheckToken()
        {
            if (User.FindFirst("UserId") == null || User.FindFirst("PermissionEvents") == null || User.FindFirst("EventId") == null)
                return ResponseBadRequest("Faild");
            string userID = User.FindFirst("UserId").Value;
            string permissions = User.FindFirst("PermissionEvents").Value;
            string eventID = User.FindFirst("EventId").Value;
            string permissionDefault = User.FindFirst("PermissionDefault").Value;
            var result = userSupportService.CheckRoleEvent(userID, eventID, permissions, false, permissionDefault);
            if (result.data is null)
                return ResponseBadRequest(result.message, result.code);
            return ResponseOk(result.data, result.code);
        }


        [Authorize]
        [HttpPost("getTokenOpenViduLandingPage")]
        public IActionResult GetTokenOpenViduLandingPage([FromBody] GetTokenOpenViduLandingPageRequest request)
        {
            if (User.FindFirst("UserId") == null || User.FindFirst("PermissionEvents") == null || User.FindFirst("EventId") == null)
                return ResponseBadRequest("Faild");
            string userID = User.FindFirst("UserId").Value;
            string permissions = User.FindFirst("PermissionEvents").Value;
            string eventID = User.FindFirst("EventId").Value;
            string permissionDefault = User.FindFirst("PermissionDefault").Value;
            var result = userSupportService.GetTokenViduLandingPage(userID, eventID, permissions, request.TypeVidu);
            if (result.data is null)
                return ResponseBadRequest(result.message, result.code);
            return ResponseOk(result.data, result.code);
        }



        //[Authorize]
        [HttpPost("removeVidu")]
        public IActionResult RemoveVidu([FromBody] TokenVidu request)
        {
            if (User.FindFirst("UserId") == null || User.FindFirst("PermissionEvents") == null || User.FindFirst("EventId") == null)
                return ResponseBadRequest("Faild");
            string userID = User.FindFirst("UserId").Value;
            string permissions = User.FindFirst("PermissionEvents").Value;
            string eventID = User.FindFirst("EventId").Value;
            string permissionDefault = User.FindFirst("PermissionDefault").Value;
            var result = userSupportService.RemoveRoomVidu(userID, eventID, permissions, false, permissionDefault, request.Token);
            if (result.data is null)
                return ResponseBadRequest(result.message, result.code);
            return ResponseOk(result.data, result.code);
        }


        [Authorize]
        [HttpGet("getTokenOpenViduCMS")]
        public IActionResult CheckTokenCMS()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            if (User.FindFirst("UserId") == null || User.FindFirst("PermissionDefault") == null)
                return ResponseBadRequest("Faild", "Error");
            string userID = User.FindFirst("UserId").Value;
            string permissions = User.FindFirst("PermissionEvents").Value;
            string permissionDefault = User.FindFirst("PermissionDefault").Value;
            var result = userSupportService.CheckRoleEvent(userID, idEventHeader, permissions, true, permissionDefault);
            if (result.data is null)
                return ResponseBadRequest(result.message, result.code);
            return ResponseOk(result.data, result.code);
        }


        //[Authorize]
        [HttpPost("removeViduCMS")]
        public IActionResult RemoveViduCMS([FromBody] TokenVidu request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            if (User.FindFirst("UserId") == null || User.FindFirst("PermissionDefault") == null)
                return ResponseBadRequest("Có lỗi xảy ra.", "Error");
            string userID = User.FindFirst("UserId").Value;
            string permissions = User.FindFirst("PermissionEvents").Value;
            string permissionDefault = User.FindFirst("PermissionDefault").Value;
            var result = userSupportService.RemoveRoomVidu(userID, idEventHeader, permissions, true, permissionDefault, request.Token);
            if (result.data is null)
                return ResponseBadRequest(result.message, result.code);
            return ResponseOk(result.data, result.code);
        }

        [HttpPost("inviteSpeak")]
        public IActionResult InviteSpeak([FromBody] InviteSpeakRequest request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess");
            }
            string eventId = Request.Headers["Event-Id"].ToString();
            string token = Request.Headers["Authorization"].ToString();

            (object data, string message) = userSupportService.InviteSpeak(eventId, token, request);

            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

        [Authorize]
        [HttpPost("confirmSpeak")]
        public IActionResult ConfirmSpeak([FromBody] ConfirmInviteSpeakRequest request)
        {
            string eventId = User.FindFirst("EventId")?.Value;
            string token = Request.Headers["Authorization"].ToString();
            string userId = User.FindFirst("UserId")?.Value;

            request.UserId = userId;
            (object data, string message) = userSupportService.ConfirmInviteSpeak(eventId, token, request);

            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

    }
}