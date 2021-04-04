using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Database;
using Project.Modules.Groups.Requests;
using Project.Modules.Groups.Entities;
using Project.App.Middleware;
using Microsoft.AspNetCore.Authorization;
using Project.Modules.Groups.Services;
using System.Text;
using System.Security.Claims;
using Project.App.Requests;
using Project.Modules.Question.Validation;

namespace Project.Modules.Groups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //  [Authorize(Roles = "ADMINISTRATOR_SYSTEM")]
    //  [MiddlewareFilter(typeof(CheckTokenMiddleware))]
    public class GroupController : BaseController
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly IGroupService groupService;

        public GroupController(MariaDBContext mariaDBContext, IGroupService groupService)
        {
            this.mariaDBContext = mariaDBContext;
            this.groupService = groupService;
        }
        
        [Authorize]
        [DisplayNameAttribute(Modules = 8, Level = 1)]
        [HttpPost("store")]
        public IActionResult Store([FromBody] StoreGroupRequest data)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                    return ResponseForbidden("EventNotExist");
            string userId = User.FindFirstValue("UserID").ToString();
            data.EventId = Request.Headers["Event-Id"].ToString();
            (Group group, string message) = groupService.Store(data, userId);
            if( group is null)
            {
                return ResponseBadRequest(message);
            }    
            return ResponseOk(group,message);
        }

        [DisplayNameAttribute(Modules = 8, Level = 8)]
        [HttpPut("update/{groupId}")]
        public IActionResult Update([FromBody] UpdateGroupRequest data, int groupId)
        {
            (Group group, string message) = groupService.Update(groupId, data);
            if (group is null)
                return ResponseBadRequest(message);
            return ResponseOk(group,message);
        }

        [DisplayNameAttribute(Modules = 8, Level = 16)]
        [HttpDelete("delete/{GroupID}")]
        public IActionResult Delete(int GroupID)
        {
            (Group group, string message) = groupService.Delete(GroupID);
            if (group is null)
                return ResponseBadRequest(message);
            return ResponseOk(group, message);
        }

        [DisplayNameAttribute(Modules = 8, Level = 2)]
        [HttpPost("show-group-by-event")]
        public IActionResult ShowAll(string eventId, [FromBody] RequestTable request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseForbidden("Tài khoản không có quyền truy cập");
            eventId = Request.Headers["Event-Id"].ToString();
            (ResponseTable response, string message) = groupService.ShowByEvent(eventId, request);
            if (response is null && message.Equals("EventNotExist"))
            {
                return ResponseBadRequest(message);

            }
            return ResponseOk(response,message);
        }
    
        [DisplayNameAttribute(Modules = 8, Level = 1)]
        [HttpPut("copy-group/{groupId}")]
        public IActionResult Copy( int groupId)
        {
            var (group, message) = groupService.Copy(groupId);
            if (group is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(group, message);
        }   
        [HttpPost("update-order")]
        public IActionResult UpdateOrder([FromBody] OrderRequest orderRequest)
        {
            (List<Group> group, string message) = groupService.UpdateOrder(orderRequest);
            if(group is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(group);
        }

        [HttpPost("list-user-show-group-by-event")]
        public IActionResult ShowAllListUser(string eventId, [FromBody] RequestTable request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseForbidden("AccountNotPermissionAccess");
            eventId = Request.Headers["Event-Id"].ToString();
            (ResponseTable response, string message) = groupService.ShowByEvent(eventId, request);
            if (response is null && message.Equals("EventNotExist"))
            {
                return ResponseBadRequest(message);

            }
            return ResponseOk(response,message);
        }
    }
}