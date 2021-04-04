using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Services;
using Project.Modules.Users.Validations;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Middlewares;
using static Project.Modules.Users.Middlewares.CheckPermissonUserMiddleware;
using Project.App.Helpers;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IUserPermissionService _userPermissionService;
        private readonly IPermissionService _permissionService;
        public UsersController(IUserService userService, IUserPermissionService userPermissionService, IPermissionService permissionService)
        {
            _userService = userService;
            _userPermissionService = userPermissionService;
            _permissionService = permissionService;
        }


        //[Authorize(Roles = "USER_CREATE")]
        //[MiddlewareFilter(typeof(CheckPermissonUserMiddlewareAttribute))]
        [HttpPost]
        public IActionResult Store([FromBody] StoreUserRequest input)
        {

            string userId = User.FindFirst("user_id").Value;
            (User checkUser, string messageUser) = _userService.ShowDetail(userId);
            if (checkUser is null)
            {
                return ResponseUnauthorized("AccountNotExist");
            }
            if (checkUser.UserLevel != UserLevelEnum.SUPERADMIN &&(!string.IsNullOrEmpty(input.GroupId))&& !checkUser.GroupId.Equals(input.GroupId))
            {
                return ResponseForbidden("Unauthorized");
            }
            if(checkUser.UserLevel == UserLevelEnum.USER)
            {
                input.GroupId = User.FindFirst("group_id").Value;
            }    
            (User data, string message) = _userService.Store(input);
            if (data == null) return ResponseBadRequest(message);
            return ResponseOk(data, message);

        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Store([FromBody] LoginRequest input)
        {
            (object data, string message) = _userService.Login(input);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }

        [HttpGet("logout")]
        [AllowAnonymous]
        public IActionResult Logout()
        {
            string accessToken;
            try
            {
                accessToken = HttpContext.Request.Headers["Authorization"].ToString().Substring(7);
                (object result, string message) = _userService.Logout(accessToken);
                if (result is null)
                {
                    return ResponseUnauthorized(message);
                }
                return ResponseOk(message);
            }
            catch
            {
                return ResponseUnauthorized("TokenRequied");
            }


        }


        [HttpGet("byLevel/{level}")]
        public IActionResult ShowAll(UserLevelEnum level)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (User user, string messageUser) = _userService.ShowDetail(userId);
            if (user is null)
            {
                return ResponseUnauthorized("AccountNotExist");
            }
            (List<User> result2, _) = _userService.ShowAll(user.GroupId, level);
            return ResponseOk(result2);
        }

        [HttpGet]
        public IActionResult ShowAllCustomPaganation([FromQuery] PaginationRequest request)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (User checkUser, string messageUser) = _userService.ShowDetail(userId);
            if (checkUser is null)
            {
                return ResponseUnauthorized("AccountNotExist");
            }
            var result = _userService.ShowTable(request, checkUser?.GroupId, checkUser.UserLevel);
            return ResponseOk(result);
        }

        [HttpGet("Profile")]
        public IActionResult Me()
        {
            string userId = User.FindFirst("user_id").Value;
            (object user, string message) = _userService.ShowDetail(userId);
            if (user is null) return ResponseBadRequest(message);
            return ResponseOk(user, message);
        }

        [HttpGet("{idUser}")]
        public IActionResult Show(string idUser)
        {
            (object user, string message) = _userService.ShowDetail(idUser);
            if (user is null) return ResponseBadRequest(message);
            return ResponseOk(user, message);
        }

        /// <summary>
        /// Update me
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut()]
        public IActionResult UpdateMe([FromForm] UpdateProfileRequest request)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (object result, string message) = _userService.EditProfile(request, userId);
            if (result == null) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }


        [HttpPut("{idUser}")]
        public IActionResult Update(string idUser, [FromBody] UpdateUserRequest request)
        {
            (object result, string message) = _userService.Edit(request, idUser);
            if (result == null) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }
        [HttpPut("Active/{idUser}")]
        public IActionResult Active(string idUser)
        {
            (User user, string message) = _userService.EditStatus(idUser);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);

        }
        [HttpPut("exptendTime/{idUser}")]
        public IActionResult Exptend(string idUser, [FromBody] ExtendRequest request)
        {
            (User user, string message) = _userService.EditExpired(request, idUser);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }

        //[Authorize(Roles = "USER_DELETE")]
        //[MiddlewareFilter(typeof(CheckPermissonUserMiddlewareAttribute))]
        [HttpDelete("{idUser}")]
        public IActionResult Delete(string idUser)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (User checkUser, string messageUser) = _userService.ShowDetail(userId);
            if (checkUser is null)
            {
                return ResponseUnauthorized("AccountNotExist");
            }
            (User user, string message) = _userService.ShowDetail(idUser);
            if (!(checkUser is not null && user is not null)) return ResponseBadRequest(messageUser);
            if (checkUser.UserLevel != UserLevelEnum.SUPERADMIN && (!user.GroupId.Equals(checkUser.GroupId)))
            {
                return ResponseForbidden(" ");
            }
            (bool flag, string message) DeleteUser = _userService.DeleteUser(idUser);
            if (!DeleteUser.flag)
                return ResponseBadRequest(DeleteUser.message);
            return ResponseOk(null, DeleteUser.message);
        }

        /// <summary>
        /// 1 =>Token level User - Chỉ xóa được trong group nó  ||
        /// 2 =>Token level SuperAdmin - Xóa được cả user lẫn SuperAdmin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteRange([FromBody] DeleteUserRequest request)
        {
            int level = int.Parse(User.FindFirst("is_level").Value.ToString());
            string groupId = User.FindFirst("group_id").Value.ToString();
            if (string.IsNullOrEmpty(groupId))
            {
                groupId = null;
            }
            (bool result, string message) = _userService.DeleteRangeUser(request, (UserLevelEnum)level, groupId);
            if (result)
            {
                return ResponseOk(message, "DeleteListUserSuccess");
            }
            return ResponseBadRequest(message);
        }



        //[Authorize(Roles = "USER_PERMISSON")]
        [HttpGet("permission/{idUser}")]
        public IActionResult GetPermissionUser(string idUser)
        {
            (object data, string message) = _userPermissionService.GetPermissionUser(idUser);
            return ResponseOk(data, message);
        }


        /// <summary>
        /// Cập nhật permission : Gửi tất cả quyền của user, quyền cũ không gửi lên = xóa 
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = "USER_PERMISSON")]
        //[MiddlewareFilter(typeof(CheckPermissonUserMiddlewareAttribute))]
        [HttpPut("updatePermission/{idUser}")]
        public IActionResult UpdatePermissionUser(string idUser, [FromBody] UpdatePermissionUserRequest request)
        {
            (object data, string message) = _userPermissionService.UpdatePermissionUser(idUser, request);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }

        [HttpPost("forgot")]
        [AllowAnonymous]
        public IActionResult ForgotPassword([FromBody] ForgotRequest request)
        {
            (User user, string message) = _userService.SendMailForgotPassword(request.Email);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(new { Email = user.UserEmail }, message);
        }

        [HttpPut("resetForgotPassword/key={CodeOtp}")]
        [AllowAnonymous]
        public IActionResult Reset(string CodeOtp, [FromBody] ResetPasswordRequest request)
        {
            (User user, string message) = _userService.ResetPassword(CodeOtp, request);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(new { Email = user.UserEmail }, message);
        }
        [HttpPut("changePassword")]
        public IActionResult ChangePassword([FromBody] ResetPasswordRequest request)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (User checkUser, string messageUser) = _userService.ShowDetail(userId);
            if (checkUser is null)
            {
                return ResponseUnauthorized("AccountNotExist");
            }
            (User user, string message) = _userService.ChangePassword(request, userId);
            if (user is null)
            {
                return ResponseBadRequest(message);
            }
            else
            {
                return ResponseOk(user.UserName, message);
            }
        }

        [HttpGet("inGroup")]
        public IActionResult ShowUserInGroupByToken()
        {
            string groupId = User.FindFirst("group_id").Value.ToString();
            (List<User> data, string message) = _userService.ShowAll(groupId, UserLevelEnum.USER);
            return ResponseOk(data.Select(x => new User() { UserId = x.UserId, UserName = x.UserName, UserEmail = x.UserEmail, UserStatus = x.UserStatus }), message);
        }
         [HttpPut("actives")]
        public IActionResult ActiveList([FromBody] DeleteUserRequest request)
        {
            (object data, string message)  = _userService.UpdateStatus(request.UserIds,UserStatusEnum.ACTIVE);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPut("unActives")]
        public IActionResult UnActiveList([FromBody] DeleteUserRequest request)
        {
            (object data, string message) = _userService.UpdateStatus(request.UserIds, UserStatusEnum.DEACTIVE);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
    }
}