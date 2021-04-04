using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Project.App.Controllers;
using Project.App.Requests;
using Project.Modules.Organizes.Entities;
using Project.Modules.Organizes.Requests;
using Project.Modules.Organizes.Services;
using Project.Modules.Question.Validation;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;

namespace Project.Modules.Organizes.Controllers
{
    [Route("api/organize-detail")]
    [ApiController]
    [DisplayNameAttribute]

    public class OrganizeDetailController : BaseController
    {
        private readonly IEventUserService eventUserService;
        public OrganizeDetailController(IEventUserService organizaDetailService)
        {
            this.eventUserService = organizaDetailService;
        }

        [DisplayNameAttribute(Modules =5, Level = 1)]
        [HttpPost("add-user-to-event")]
        public IActionResult AddUser([FromBody] AddUserToEventRequest request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseForbidden("EventNotExist");
            request.EventId = Request.Headers["Event-Id"].ToString();
            (EventUser result, string message)  = eventUserService.AddUserToEvent(request);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }

        [DisplayNameAttribute(Modules = 5, Level = 8)]
        [HttpPut("edit-user-to-event/{eventUserId}")]
        public IActionResult EditUser(int eventUserId, [FromBody] UpdateUserToEventRequest request)
        {
            (EventUser result, string message) = eventUserService.UpdateUserToEvent(request, eventUserId);
            if (result is null && !(message.Equals("success")))
                {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result,message);
        }

        [DisplayNameAttribute(Modules = 5, Level = 16)]
        [HttpDelete("delete-user-in-event/{eventUserId}")]
        public IActionResult RemoveUser(int eventUserId)
        {
            (EventUser result, string message)= eventUserService.DeleteUserInEvent(eventUserId);
            if(result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }

        [DisplayNameAttribute(Modules = 5, Level = 2)]
        [HttpPost("show-user-by-event")]
        public IActionResult ShowUserByEvent([FromBody] RequestTable request)
        {
            string organizeID = User.FindFirst("OrganizeId").Value;
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseForbidden("EventNotExist");
            string eventId = Request.Headers["Event-Id"].ToString();
            var (response, message) = eventUserService.ShowUsersByEvent(eventId, request, organizeID);
            if(response is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(response, message);
        }

        [HttpPut("block-user-in-event/{eventUserId}")]
        public IActionResult BlockUser(int eventUserId)
        {
            (EventUser result, string message)  = eventUserService.BlockUserInEvent(eventUserId, (int)EventUser.STATUS.BLOCK);
            if(result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result,message);
        }
        
        [HttpPut("un-block-user-in-event/{eventUserId}")]
        public IActionResult UnBlockUser(int eventUserId)
        {
            (EventUser result, string message) = eventUserService.BlockUserInEvent(eventUserId, (int)EventUser.STATUS.ACTIVE);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result,message);
        }

        [DisplayNameAttribute(Modules = 5, Level = 2)]
        [HttpPost("filter-shareholder-code")]
        public IActionResult FilterUser([FromBody] FilterShareholderCode filter)
        {
            string organizeId = User.FindFirstValue("OrganizeId");
           
            if(String.IsNullOrEmpty(organizeId))
            {
                return ResponseBadRequest("AdminNotExistInOrganize");
            }
            filter.OrganizeId = organizeId;
            (User user, string message)  = eventUserService.FilterUser(filter);
           
            return ResponseOk(user, message);
        }
        [HttpGet("list-user-none-stocks")]
        public IActionResult ShowUserNoneStocks()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseForbidden("EventNotExist");
            string eventId = Request.Headers["Event-Id"].ToString();
            (List<User> users, string message) = eventUserService.UserNoneStocks(eventId);
            if(users is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(users, message);
        }
        [HttpGet("getListUserOrganize")]
        public IActionResult GetUserByOrganize()
        {
            string oranizeID = User.FindFirst("OrganizeId").Value;
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseForbidden("EventNotExist");
            string eventID = Request.Headers["Event-Id"].ToString();
            var result = eventUserService.GetListUserFromOrganize(oranizeID, eventID);
            return ResponseOk(result, "ShowListSuccess", "listuserorganize");
        }


        [HttpPost("list-user-on-start-event")]
        public IActionResult GetListUserOnStartEvent([FromBody] RequestTable request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseForbidden("EventNotExist");
            string eventID = Request.Headers["Event-Id"].ToString();
            (ResponseTable response, string message)  = eventUserService.ShowUsersStartEvent(eventID, request);
            if (response is null)
                return ResponseBadRequest(message);
            if (response.DateResult is null)
            {
                return ResponseOk(response, message, "listUserOnStartEvent");

            }
            return ResponseOk(response,message, "listUserOnStartEvent");
        }

        [DisplayNameAttribute(Modules = 12, Level = 2)]
        [HttpPost("list-user-login-event")]
        public IActionResult GetListUserLoginEvent( [FromBody] RequestTable request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseForbidden("EventNotExist");
            string eventID = Request.Headers["Event-Id"].ToString();
            (ResponseTable response, string message) = eventUserService.ShowUsersLoginEvent(eventID, request);
            if ((response is null) && !message.Equals("success"))
                return ResponseBadRequest(message);
            if (response.DateResult is null)
            {
                return ResponseOk(response, message, "listUserLoginEvent");

            }
            return ResponseOk(response, message, "listUserOnStartEvent");
        }

        [HttpGet("logout-landing")]
        public IActionResult LogOutLanding()
        {
            string userId = User.FindFirstValue("UserId");
            string eventId = User.FindFirstValue("EventId");
            var (status, message, code) = eventUserService.LogOutLanding(eventId,userId);
            if ( status ==false)
            {
                return ResponseBadRequest(message, code);
            }
            return ResponseOk(status, message, code);
        }

        [DisplayNameAttribute(Modules = 5, Level = 1)]
        [HttpPost("import-user-to-event")]
        public IActionResult ImportUsers([FromBody] ImportRequest request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseForbidden("EventNotExist");
            request.EventId = Request.Headers["Event-Id"].ToString();
            (List<ItemImportUserToEvent> result, string message)  = eventUserService.ImportUserInEvent(request);

            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            int successs = result.Count(m => m.ImportStatus == true);
            if (successs ==0)
            {
                return ResponseBadRequest("ImportUserToEventFaild");
            }
            return ResponseOk(result,message);
        }

    }
}