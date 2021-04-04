using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Controllers;
using Project.App.Requests;
using Project.Modules.Invitations.Entities;
using Project.Modules.Invitations.Request;
using Project.Modules.Invitations.Service;
using Project.Modules.Question.Validation;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Project.Modules.Invitations.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class TemplateController : BaseController
    {
        #region Prop
        private readonly IInvitationService invitationService;
        #endregion
        #region Constructor
        public TemplateController(
            IInvitationService InvitationService
            )
        {
            invitationService = InvitationService;
        }
        #endregion
        #region Act
        [DisplayNameAttribute(Modules=4, Level =2)]
        [HttpPost("event/{eventId}/template")]
        public IActionResult ListTemplate([FromBody] RequestTable request)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            return ResponseOk(invitationService.ListTemplate(eventId, request), "Hiển thị danh sách biểu mẫu", "ShowListTemplateSuccess");
        }

        [HttpPost("event/{eventId}/template/preview")]
        public IActionResult PreviewTemplate([FromBody]JObject jObject)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            string userId = User.FindFirstValue("UserID");
            if(!int.TryParse(jObject["type"].ToString(), out int type))
            {
                return ResponseBadRequest("Loại biểu mẫu sai định dạng");
            }

            (bool check, string data) = invitationService.PreviewTemplate(eventId, jObject["template"]?.ToString(), userId, type);

            if (!check)
            {
                return ResponseBadRequest(data);
            }
                
            return ResponseOk(data, "Xem trước biểu mẫu", "ShowDetailTemplateSuccess");
        }

        [HttpGet("event/{eventId}/template/{templateId}")]
        public IActionResult DetailTemplate(string templateId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            Template template = invitationService.DetailTemplate(templateId, eventId);
            if(template is null)
            {
                return ResponseBadRequest("Không tìm thấy biểu mẫu", "TemplateNotFound");
            }

            return ResponseOk(template, "Hiển thị chi tiết", "ShowDetailTemplateSuccess");
        }
        [DisplayNameAttribute(Modules = 4, Level = 1)]
        [HttpPost("event/{eventId}/template/add")]
        public IActionResult AddTemplate(AddTemplateRequest request)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            string userId = User.FindFirstValue("UserID");
            string userType = User.FindFirstValue("PermissionDefault");

            Template template = new Template
            {
                TemplateTitle = request.Title,
                TemplateContent = request.Content,
                TemplateType = request.Type.Value,
                EventId = eventId,
                UserId = userId
            };
            (Template data, string message) = invitationService.AddTemplate(template, userId, userType);
            if(template is null)
            {
                return ResponseBadRequest(message);
            }
                
            return ResponseOk(data, "Thêm biểu mẫu thành công", "AddTemplateSuccess");
        }

        [DisplayNameAttribute(Modules = 4, Level = 8)]
        [HttpPut("event/{eventId}/template/{templateId}")]
        public IActionResult EditTemplate(EditTemplateRequest request, string templateId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            Template template = invitationService.DetailTemplate(templateId, eventId);
            if(template is null)
            {
                return ResponseBadRequest("Không tìm thấy biểu mẫu", "TemplateNotFound");
            }

            if (!string.IsNullOrEmpty(request.Title))
            {
                template.TemplateTitle = request.Title;
            }

            if (request.Content != null)
            {
                template.TemplateContent = request.Content;
            }

            if(request.Type != null)
            {
                template.TemplateType = request.Type.Value;
            }

            if (request.Status != null)
            {
                template.TemplateStatus = request.Status.Value;
            }

            template = invitationService.EditTemplate(template);
            return ResponseOk(template, "Sửa biểu mẫu thành công", "EditTemplateSuccess");
        }

        [DisplayNameAttribute(Modules=4, Level =16)]
        [HttpDelete("event/{eventId}/template/{templateId}")]
        public IActionResult DeleteTemplate(string templateId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            Template template = invitationService.DetailTemplate(templateId, eventId);
            if (template is null)
            {
                return ResponseBadRequest("Không tìm thấy biểu mẫu", "TemplateNotFound");
            }

            template.DeletedAt = DateTime.Now;
            _ = invitationService.EditTemplate(template);
            return ResponseOk(null, "Xóa biểu mẫu thành công", "DeleteTemplateSuccess");
        }

        [HttpPost("event/{eventId}/invitation")]
        public IActionResult AddInvitation([FromBody] JToken jtoken)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            string jUser = JsonConvert.SerializeObject(jtoken["users"]);
            List<string> users = JsonConvert.DeserializeObject<List<string>>(jUser);
            (bool check, string message) = invitationService.AddInvitation(eventId, users);
            if (!check)
            {
                return ResponseBadRequest(message);
            }
                
            return ResponseOk(null, "Thêm thư mời thành công");
        }

        [HttpPost("event/{eventId}/sendMail")]
        public IActionResult SendMailInvitation([FromBody] JToken jtoken)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            string jUser = JsonConvert.SerializeObject(jtoken["users"]);
            List<string> users = JsonConvert.DeserializeObject<List<string>>(jUser);
            (bool check, string message) = invitationService.SendMailInvitation(eventId, users);
            if (!check)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(null, "Gửi thư mời thành công");
        }

        [HttpPost("event/{eventId}/getInvitation")]
        public IActionResult GetInvitation([FromBody] JToken jtoken)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            string jUser = JsonConvert.SerializeObject(jtoken["users"]);
            List<string> users = JsonConvert.DeserializeObject<List<string>>(jUser);
                
            (bool check, string message, List<object> data) = invitationService.GetMailInvitation(eventId, users);
            if (!check)
            {
                return ResponseBadRequest(message);
            }
                
            return ResponseOk(data, message);
        }

        [DisplayNameAttribute(Modules = 10, Level = 2)]
        [HttpPost("reportEndMeeting/{eventId}")]
        public IActionResult ReportEndMeeting()
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            (bool check, string message) = invitationService.ShowReportEndMeeting(eventId);
            if (!check)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(message, "Lấy báo cáo thành công");
        }
        #endregion
    }
}