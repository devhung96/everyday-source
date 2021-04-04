using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Groups.Entities;
using Project.Modules.Groups.Requests;
using Project.Modules.Groups.Sevices;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Services;

namespace Project.Modules.Groups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : BaseController
    {
        private readonly IGroupService groupService;
        private readonly IUserService userService; 
        public GroupController(IGroupService groupService, IUserService userService)
        {
            this.groupService = groupService;
            this.userService = userService;
        }
        [Authorize]
        [HttpGet]
        public IActionResult ShowTable([FromQuery] PaginationRequest request)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (User user, string message) = userService.ShowDetail(userId);
            if(user is null)
            {
                return ResponseForbidden(message);
            }
            if(user.UserLevel == UserLevelEnum.SUPERADMIN)
            {
                PaginationResponse<Group> data = groupService.ShowTable(request, null,true);
                return ResponseOk(data, "Success");
            }
            else
            {
                PaginationResponse<Group> data = groupService.ShowTable(request, user.GroupId,false);
                return ResponseOk(data, "Success");
            } 
        } 
        /// <summary>
        /// Chỉ supperAdmin và user trong group mới thấy được user trong Group đó
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("users/{groupId}")]
        public IActionResult ShowUserTable([FromQuery] PaginationRequest request, string groupId)
        {
            string groupIdToken = User.FindFirst("group_id").Value.ToString();
            UserLevelEnum userLevel = (UserLevelEnum)int.Parse(User.FindFirst("is_level").Value.ToString());
            if( string.IsNullOrEmpty(groupId) && groupId.Equals(groupIdToken) && userLevel != UserLevelEnum.SUPERADMIN)
            {
                return ResponseForbidden("NotInGroup");
            }
            PaginationResponse<User> data = groupService.ShowUserInGroup(request, groupId);
            return ResponseOk(data,"Success");
        }
        [Authorize]
        [HttpPut("extendTime/{groupId}")]
        public IActionResult ShowUserTable([FromBody] ExtendGroupRequest request,string groupId)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (object data, string message) = groupService.ExtendTime(request, groupId, userId);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data,"Success");
        }
        
        [HttpPut("{groupId}")]
        public IActionResult UpdateGroup([FromForm] UpdateGroupRequest request, string groupId)
        {
            (object group, string message) = groupService.Update(request,groupId);
            if (group is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(group, message);
        }  
        [HttpPut("active/{groupId}")]
        public IActionResult AcitveGroup(string groupId)
        {
            (object group, string message) = groupService.UpdateStatus(groupId);
            if (group is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(group, message);
        }
        [HttpGet("{groupId}")]
        public IActionResult Detail(string groupId)
        {
            (Group group, string message) = groupService.Detail(groupId);
            if(group is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(group);
        }
        [HttpDelete("{groupId}")]
        public IActionResult Delete(string groupId)
        {
            (object data, string message) = groupService.Delete(groupId);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }

        [HttpDelete]
        public IActionResult DeleteRange([FromBody] ListGroupRequest request)
        {
            (object data, string message) = groupService.DeleteRange(request);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Store([FromForm]StoreGroupRequest request)
        {
            UserLevelEnum userLevel = (UserLevelEnum)int.Parse(User.FindFirst("is_level").Value.ToString());
            if( userLevel != UserLevelEnum.SUPERADMIN)
            {
                return ResponseUnauthorized("Unauthorized");
            }    
            (object data, string message) = groupService.Create(request);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpGet("full")]
        public IActionResult ShowAllGroup()
        {
            return ResponseOk(groupService.ShowAll(),"Success");
        }

        [HttpPut("actives")]
        public IActionResult ActiveList([FromBody] ListGroupRequest request)
        {
            (object data, string message)  = groupService.UpdateStatus(request.GroupIds, Group.STATUS.Active);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPut("unActives")]
        public IActionResult UnActiveList([FromBody] ListGroupRequest request)
        {
            (object data, string message)  = groupService.UpdateStatus(request.GroupIds, Group.STATUS.UnActive);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
    }
}
