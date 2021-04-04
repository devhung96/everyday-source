using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Database;
using Project.App.Middleware;
using Project.App.Requests;
using Project.Modules.Authorites.Requests;
using Project.Modules.Authorities.Entities;
using Project.Modules.Authorities.Requests;
using Project.Modules.Authorities.Services;
using Project.Modules.Question.Validation;

namespace Project.Modules.Authorities.Controllers
{
    [Route("api/[controller]")]
    [DisplayNameAttribute]
    [ApiController] //BACKEND
    public class AuthorityController : BaseController
    {
        private readonly IAuthorityService authorityService;
        public AuthorityController(IAuthorityService authorityService)
        {
            this.authorityService = authorityService;
        }
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [HttpPost("vote")]
        public IActionResult StoreForVote([FromBody] StoreForVoteRequest request)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString();
            var result = authorityService.StoreForVote(request, token);
            return ResponseOk(result.data,"CreateVote");
        }
        [HttpPost("authorized")]
        public IActionResult StoreForAuthorized([FromBody] StoreForAuthority request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseBadRequest("EventNotFound");
            string eventID = Request.Headers["Event-Id"].ToString();
            request.EventID = eventID;
            string token = HttpContext.Request.Headers["Authorization"].ToString();
            return ResponseOk(authorityService.StoreForAuthority(request, token),"CreateAuthorizedSuccess");
        }
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [HttpGet("showUserAuthority/{questionID}")]
        public IActionResult ShowUserAuthority(string questionID)
        {
            if (User.FindFirst("UserId") == null || User.FindFirst("EventId") == null)
            {
                return ResponseBadRequest("Error");
            }
            string userReceiveID = User.FindFirst("UserId").Value;
            string eventID = User.FindFirst("EventId").Value;
            return ResponseOk(authorityService.ListUserAuthority(userReceiveID, eventID, questionID), "ListUserReceive");
        }
        [HttpPost("showAll/{eventID}/{questionID}")]
        public IActionResult ShowAllInEvent([FromBody] RequestTable table, string eventID, string questionID)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseBadRequest("EventCodeDoesNotExist.");
            string EventID = Request.Headers["Event-Id"].ToString();
            return ResponseOk(authorityService.ShowAllInEvent(table, eventID, questionID),"ListAuthorityEvent");
        }
        [HttpGet("showAll")]
        public IActionResult ShowAll()
        {
            return ResponseOk(authorityService.ShowAll(),"ListVoteAndAuthorized");
        }
        [HttpGet("showID/{id}")]
        public IActionResult Show(int id)
        {
            var authority = authorityService.Show(id);
            if (authority.data is null)
                return ResponseBadRequest("AuthorityNotFound");
            return ResponseOk(authority.data,"GetAuthoritySuccess");
        }
        [HttpDelete("{authorityID}")]
        public IActionResult DeleteFromEvent(int authorityID)
        {
            //if (String.IsNullOrEmpty(Request.Query["userID"].ToString()) || String.IsNullOrEmpty(Request.Query["eventID"].ToString()))
            //    return ResponseBadRequest("Có trường bị thiếu", "Somefieldsaremissing");
            //authorityService.DeleteFromEvent(Request.Query["userID"].ToString(), Request.Query["eventID"].ToString());
            //return ResponseOk("Xóa ủy quyền thành công", "Xóa ủy quyền thành công");

            string token = HttpContext.Request.Headers["Authorization"].ToString();
            var result = authorityService.DeleteAuthority(authorityID, token);
            if (!result.flag)
                return ResponseBadRequest(result.message,"DeleteError");
            return ResponseOk(result.message, result.message, "DeleteSuccess");
        }
        [HttpPut("changeStatus/{authorityID}")]
        public IActionResult ChangeStatus([FromBody] ChangeStatus request, int authorityID)
        {
            var authority = authorityService.ChangeStatus(authorityID, request);
            if (authority.data == null)
                return ResponseBadRequest(authority.message);
            return ResponseOk(authority.data);
        }
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [HttpGet("showAllVote/{eventID}")]
        public IActionResult ShowAllForVote(string eventID)
        {
            return ResponseOk(authorityService.ShowAllVote(eventID),"ListVote");
        }
        [HttpPost("showAllAuthorized/{eventID}")]
        public IActionResult ShowAllForAuthorized([FromBody] RequestTable table, string eventID)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseBadRequest("EventNotFound");
            string EventID = Request.Headers["Event-Id"].ToString();
            return ResponseOk(authorityService.ShowAllAuthorized(EventID, table), "ListAuthorized");
        }
        [HttpPut("updateAuthorityWithID")]
        public IActionResult UpdateAuthorityWithID([FromBody] UpdateForAuthorityWithID request)
        {
            var result = authorityService.UpdateAuthorityWithID(request);
            return ResponseOk(result,"UpdateAuthoritySuccess");
        }
        [HttpPut("updateAuthority")]
        public IActionResult UpdateAuthority([FromBody] UpdateAuthority request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseBadRequest("EventNotFound");
            string EventID = Request.Headers["Event-Id"].ToString();
            request.EventID = EventID;
            var result = authorityService.UpdateAuthority(request);
            if (result.data is null)
                return ResponseBadRequest(result.message, "UpdateAuthorityError");
            return ResponseOk(result.data, result.message);
        }
        [HttpPost("getUser")]
        public IActionResult GetUserForStoreAuthority([FromBody] GetUserForAuthority request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseBadRequest("EventNotFound");
            string EventID = Request.Headers["Event-Id"].ToString();
            request.EventID = EventID;
            var result = authorityService.GetUser(request.ShareHolderCode, request.EventID);
            if (result.data is null)
                return ResponseBadRequest(result.message, "GetUserError");
            return ResponseOk(result.data, "GetUserSuccess");
        }
        [HttpGet("getListUser/{eventID}/{shareHolderCode}")]
        public IActionResult GetListUserForStoreAuthority(string eventID, string shareHolderCode)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseBadRequest("EventNotFound");
            string EventID = Request.Headers["Event-Id"].ToString();
            var result = authorityService.GetListUser(EventID, shareHolderCode);
            return ResponseOk(result,"GetListUserSuccess");
        }
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [HttpGet("getUserVote")]
        public IActionResult GetListUserVote()
        {
            if (String.IsNullOrEmpty(Request.Query["eventID"].ToString()) || String.IsNullOrEmpty(Request.Query["questionID"].ToString()))
                return ResponseBadRequest("Field:enventID,questionIDIsRequired");
            var result = authorityService.GetListUserVote(Request.Query["eventID"].ToString(), Request.Query["questionID"].ToString());
            return ResponseOk(result, "GetListUserSuccess");
        }
        [HttpGet("reset")]
        public IActionResult ResetVote()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString();
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
                return ResponseBadRequest("EventNotFound");
            string EventID = Request.Headers["Event-Id"].ToString();
            if (String.IsNullOrEmpty(Request.Query["questionID"].ToString()))
                return ResponseBadRequest("Field:questionIDIsRequired");
            var result = authorityService.ResetVote(EventID, Request.Query["questionID"].ToString());
            return ResponseOk(result, result);
        }
    }
}