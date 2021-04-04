using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.App.Providers;
using Project.App.Requests;
using Project.Modules.Events.Entities;
using Project.Modules.Invitations.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.Parameters.Entities;
using Project.Modules.Reports.Services;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Invitations.Service
{
    public interface IInvitationService
    {
        ResponseTable ListTemplate(string eventId, RequestTable request);
        Template DetailTemplate(string templateId, string eventId);
        (bool, string) PreviewTemplate(string eventId, string template, string userId, int type);
        (Template, string) AddTemplate(Template template, string userId, string userType);
        Template EditTemplate(Template template);
        (bool, string) AddInvitation(string eventId, List<string> users, string templateId = null);
        (bool, string) SendMailInvitation(string eventId, List<string> users);
        (bool, string, List<object>) GetMailInvitation(string eventId, List<string> users);
        (bool, string) ShowReportEndMeeting(string eventId);
    }
    public class InvitationService : IInvitationService
    {
        #region Prop
        private readonly MariaDBContext dBContext;
        private readonly IReportService reportService;
        #endregion
        #region Constructor
        public InvitationService(
            MariaDBContext DBContext,
            IReportService ReportService
            )
        {
            dBContext = DBContext;
            reportService = ReportService;
        }
        #endregion
        #region Act
        public ResponseTable ListTemplate(string eventId, RequestTable request)
        {
            IEnumerable<Template> templates = dBContext.Templates
                .Where(x => x.DeletedAt == null && x.EventId == eventId && x.TemplateTitle.Contains(request.Search));

            ResponseTable responseTable = new ResponseTable
            {
                DateResult = request.Page == 0 ?
                            templates :
                            templates.Skip((request.Page - 1) * request.Results).Take(request.Results),
                Total = request.Page == 0 ? templates.Count() : templates.Count(),
                Info = new Info
                {
                    Page = request.Page,
                    Results = request.Results,
                    TotalRecord = templates.Skip((request.Page - 1) * request.Results).Take(request.Results).Count()
                }
            };
            return responseTable;
        }

        public Template DetailTemplate(string templateId, string eventId)
        {
            return dBContext.Templates.FirstOrDefault(x => x.DeletedAt == null && x.EventId == eventId && x.TemplateId == templateId);
        }

        private string ReplaceTemplate(string originTemplate, User user, Event events, EventUser eventUser, UserSuper userSuper = null)
        {
            string password = user != null ? eventUser?.PasswordSystem : "{{_password}}";
            if(password is null)
            {
                password = "{{Oldpassword}}";
            }

            string linkUrl = null;
            if(!string.IsNullOrEmpty(events.EventSetting))
            {
                linkUrl = JObject.Parse(events.EventSetting)["landingPage"].ToString() + $"/{events.EventId}";
            }    

            originTemplate = originTemplate.Replace("_shareholder_code", user != null ? user.ShareholderCode : "shareholder_code");
            originTemplate = originTemplate.Replace("_fullName", user != null ? user.FullName : userSuper.FullName);
            originTemplate = originTemplate.Replace("_eventId", events.EventId);
            originTemplate = originTemplate.Replace("_eventName", events.EventName);
            originTemplate = originTemplate.Replace("_eventBegin", events.EventTimeBegin.ToString());
            originTemplate = originTemplate.Replace("_eventEnd", events.EventTimeEnd.ToString());
            originTemplate = originTemplate.Replace("_link", linkUrl);
            originTemplate = originTemplate.Replace("_identity", user != null ? user.IdentityCard : "{{_identity}}");
            originTemplate = originTemplate.Replace("_password", password);
            return originTemplate;
        }

        private string ReplaceTemplateReport(string originTemplate, Event events)
        {
            List<SessionReport> sessions = dBContext.Sessions.Where(x => x.EventId.Equals(events.EventId)).OrderBy(x => x.SessionSort)
                .Select(x => new SessionReport
                {
                    Description = x.SessionDescription,
                    Title = x.SessionTitle,
                    SessionId = x.SessionId
                })
                .ToList();

            int countUser = dBContext.EventUsers.Count(x => x.EventId.Equals(events.EventId) && x.UserLoginStatus.Equals(USER_LOGIN_STATUS.ON));
            SupportExportReport supportExportReport = reportService.SupportGetInfo(events.EventId);
            originTemplate = originTemplate.Replace("_eventBegin", events.EventTimeBegin.ToString());
            originTemplate = originTemplate.Replace("_eventEnd", events.EventTimeEnd.ToString());
            originTemplate = originTemplate.Replace("_countUser", countUser.ToString());
            originTemplate = originTemplate.Replace("_sumStock", supportExportReport.shareholderAttending.ToString());
            originTemplate = originTemplate.Replace("_percentStock", (supportExportReport.percent * 100).ToString());

            #region session
            int key = 0;
            foreach (var item in sessions)
            {
                originTemplate = originTemplate.Replace($"_session_title_{key}", item.Title);
                originTemplate = originTemplate.Replace($"_session_description_{key}", item.Description);
                item.Question = reportService.SupportGetChartWithSession(item.SessionId);

                string templateQuestion = "<div>";
                int countQuestion = 1;
                foreach (var question in item.Question)
                {
                    double percent = 0;
                    int index = 0;
                    templateQuestion += $"<p><b><i>Câu hỏi {countQuestion}:</i> {question.QuestionName}</b></p>";
                    foreach (var answers in question.Answers)
                    {
                        double percentCurrent = Math.Round(answers.percent * 100, 2);
                        if(index == question.Answers.Count)
                        {
                            templateQuestion += $"<p>{answers.title} - Số cổ phần: {String.Format("{0:#,##0}", answers.stock)} - Chiếm {100 - percent}%";
                        }
                        else
                        {
                            templateQuestion += $"<p>{answers.title} - Số cổ phần: {String.Format("{0:#,##0}", answers.stock)} - Chiếm {percentCurrent}%";
                            percent += percentCurrent;
                        }
                    }
                    templateQuestion += "</p>";
                    countQuestion++;
                }

                templateQuestion += "</div>";

                originTemplate = originTemplate.Replace($"_session_question_{key}", templateQuestion);
                key++;
            }
            #endregion
            #region Question Client
            List<QuestionClientResponse> questionClients = dBContext.QuestionClients
                .Include(x => x.QuestionCommentClient)
                .Where(x => x.QuestionActive == true && x.EventId.Equals(events.EventId))
                .Select(x => new QuestionClientResponse(x))
                .ToList()
                .OrderBy(x => x.QuestionDateComment.GetValueOrDefault())
                .ToList();

            int countQuestionClient = 1;
            string templateQuestionClient = "";
            foreach (var item in questionClients)
            {
                templateQuestionClient += $"<p><b><u>Câu hỏi: {countQuestionClient}:</u> {item.QuestionContent}</b></p>";
                templateQuestionClient += $"<p><b><u>Người đặt câu hỏi: </u>{item.UserName}</b></p>";
                templateQuestionClient += $"<p><u>Trả lời: </u>{item.QuestionComment}</p>";
                templateQuestionClient += $"<p><u>Người trả lời: </u>{item.QuestionUserComment}</p>";
                templateQuestionClient += $"<p><u>Ngày trả lời: </u>{item.QuestionDateComment}</p><br>";
                countQuestionClient++;
            }
            originTemplate = originTemplate.Replace("_questionClient", templateQuestionClient);
            #endregion
            return originTemplate;
        }

        public (bool, string) PreviewTemplate(string eventId, string template, string userId, int type)
        {
            Event events = dBContext.Events.FirstOrDefault(x => x.EventId.Equals(eventId));
            if (events is null)
            {
                return (false, "Không tìm thấy sự kiện");
            }

            User user = dBContext.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (user != null)
            {
                EventUser eventUser = dBContext.EventUsers.FirstOrDefault(x => x.UserId.Equals(userId) && x.EventId.Equals(eventId));
                if(type == 0)
                {
                    return (true, ReplaceTemplate(template, user, events, eventUser));
                }
                return (true, ReplaceTemplateReport(template, events));
            }

            UserSuper userSuper = dBContext.UserSupers.FirstOrDefault(x => x.UserSuperId.Equals(userId));
            if (userSuper is null)
            {
                return (false, "Không tìm thấy người dùng");
            }

            if(type == 0)
            {
                return (true, ReplaceTemplate(template, null, events, null, userSuper));
            }
                
            return (true, ReplaceTemplateReport(template, events));
        }

        public (bool, string) ShowReportEndMeeting(string eventId)
        {
            #region Find first template
            Template template = dBContext.Templates
                .OrderByDescending(x => x.CreatedAt)
                .Where(x => x.EventId.Equals(eventId))
                .FirstOrDefault(x => x.DeletedAt == null && x.TemplateType == 1); // loại thư mời
            #endregion
            Event events = dBContext.Events.FirstOrDefault(x => x.EventId.Equals(eventId));
            if (events is null)
            {
                return (false, "Không tìm thấy sự kiện");
            }

            if (template is null)
            {
                return (false, "Không tìm thấy biểu mẫu");
            }

            return (true, ReplaceTemplateReport(template.TemplateContent, events));
        }

        public (Template, string) AddTemplate(Template template, string userId, string userType)
        {
            Parameter parameter = dBContext.Parameters.FirstOrDefault(x => x.ParameterKey.Equals("TemplateType"));
            if (parameter is null)
            {
                return (null, "Loại biểu mẫu không tìm thấy");
            }

            JArray jParam = JArray.Parse(parameter.ParameterValue);
            JToken jToken = jParam.Where(x => x["key"].ToString().Equals(template.TemplateType.ToString())).FirstOrDefault();
            if (jToken is null)
            {
                return (null, "Loại biểu mẫu không hợp lệ");
            }

            template.TemplateTypeName = jToken["name"].ToString();

            string userName = null;
            if (userType == "ADMIN")
            {
                userName = dBContext.UserSupers.Where(x => x.UserSuperId.Equals(userId)).FirstOrDefault()?.FullName;
            }
            else
            {
                userName = dBContext.Users.Where(x => x.UserId.Equals(userId)).FirstOrDefault()?.FullName;
            }

            template.UserName = userName ?? "";
            dBContext.Templates.Add(template);
            dBContext.SaveChanges();
            return (template, "Thành công");
        }

        public Template EditTemplate(Template template)
        {
            dBContext.Templates.Update(template);
            dBContext.SaveChanges();
            return template;
        }

        public (bool, string) AddInvitation(string eventId, List<string> users, string templateId = null)
        {
            #region Find first template
            if (templateId == null)
            {
                Template template = dBContext.Templates
                    .OrderByDescending(x => x.CreatedAt)
                    .Where(x => x.EventId.Equals(eventId))
                    .FirstOrDefault(x => x.DeletedAt == null && x.TemplateType == 0 && x.TemplateStatus == TEMPLATE_STATUS.ACTIVE); // loại thư mời

                if (template is null)
                {
                    return (false, "Không tìm thấy biểu mẫu thư mời");
                }

                templateId = template.TemplateId;
            }

            #endregion
            List<EventUser> eventUsers = dBContext.EventUsers.Where(x => x.EventId.Equals(eventId)).ToList();
            List<Invitation> invitations = new List<Invitation>();

            if(users is null)
            {
                foreach (var item in eventUsers)
                {
                    invitations.Add(new Invitation
                    {
                        EventId = eventId,
                        UserId = item.UserId,
                        TemplateId = templateId
                    });
                }
            }
            else
            {
                foreach (var item in users)
                {
                    if (!eventUsers.Select(x => x.UserId).Contains(item))
                    {
                        return (false, "Người dùng không nằm trong sự kiện");
                    }

                    invitations.Add(new Invitation
                    {
                        EventId = eventId,
                        UserId = item,
                        TemplateId = templateId
                    });
                }
            }

            dBContext.Invitations.AddRange(invitations);
            dBContext.SaveChanges();
            return (true, "Success");
        }
        public (bool, string) SendMailInvitation(string eventId, List<string> users)
        {
            #region Find first template
            Template template = dBContext.Templates
                .OrderByDescending(x => x.CreatedAt)
                .Where(x => x.EventId.Equals(eventId))
                .FirstOrDefault(x => x.DeletedAt == null && x.TemplateType == 0 && x.TemplateStatus == TEMPLATE_STATUS.ACTIVE); // loại thư mời
            if (template is null)
            {
                return (false, "Không tìm thấy mẫu thư mời");
            }

            #endregion
            Event events = dBContext.Events.FirstOrDefault(x => x.EventId.Equals(eventId));
            if (events is null)
            {
                return (false, "Không tìm thấy sự kiện");
            }

            List<EventUser> eventUsers = dBContext.EventUsers.Where(x => x.EventId.Equals(eventId)).ToList();

            if (users is null)
            {
                foreach (var item in eventUsers)
                {
                    User user = dBContext.Users.FirstOrDefault(x => x.UserId.Equals(item.UserId));
                    if (user is null)
                    {
                        return (false, "Không tìm thấy người dùng");
                    }

                    string templateStr = template.TemplateContent;

                    TransportPatternProvider.Instance.Emit("SendEmail", new SendMailRequest
                    {
                        MessageSubject = template.TemplateTitle,
                        MessageContent = ReplaceTemplate(templateStr, user, events, item, null),
                        Contacts = new List<SendMailContact> 
                        { 
                            new SendMailContact{ContactEmail = user.UserEmail}
                        } 
                    });
                    System.Threading.Thread.Sleep(2000);
                }
            }
            else
            {
                foreach (var item in users)
                {
                    User user = dBContext.Users.FirstOrDefault(x => x.UserId.Equals(item));
                    if (user is null)
                    {
                        return (false, "Không tìm thấy người dùng");
                    }
                    EventUser eventUser = dBContext.EventUsers.FirstOrDefault(x => x.UserId.Equals(item) && x.EventId.Equals(eventId));
                    string templateStr = template.TemplateContent;

                    TransportPatternProvider.Instance.Emit("SendEmail", new SendMailRequest
                    {
                        MessageSubject = template.TemplateTitle,
                        MessageContent = ReplaceTemplate(templateStr, user, events, eventUser, null),
                        Contacts = new List<SendMailContact>
                        {
                            new SendMailContact{ContactEmail = user.UserEmail}
                        }
                    });
                    System.Threading.Thread.Sleep(2000);
                }
            }

            return (true, "Gửi thư mời thành công");
        }

        public (bool, string, List<object>) GetMailInvitation(string eventId, List<string> users)
        {
            #region Find first template
            Template template = dBContext.Templates
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault(x => x.DeletedAt == null && x.TemplateType == 0 && x.EventId.Equals(eventId) && x.TemplateStatus == TEMPLATE_STATUS.ACTIVE); // loại thư mời

            if (template is null)
            {
                return (false, "Không tìm thấy mẫu thư mời", null);
            }
            #endregion

            List<object> reviewMail = new List<object>();

            Event events = dBContext.Events.FirstOrDefault(x => x.EventId.Equals(eventId));
            if (events is null)
            {
                return (false, "Không tìm thấy sự kiện", null);
            }

            if (users is null)
            {
                List<EventUser> eventUsers = dBContext.EventUsers.Where(x => x.EventId.Equals(eventId)).ToList();
                foreach (var item in eventUsers)
                {
                    User user = dBContext.Users.FirstOrDefault(x => x.UserId.Equals(item.UserId));
                    if (user is null)
                    {
                        return (false, "Không tìm thấy người dùng", null);
                    }
                    string templateStr = template.TemplateContent;

                    reviewMail.Add(new
                    {
                        Key = user.ShareholderCode,
                        Value = ReplaceTemplate(templateStr, user, events, item, null)
                    });
                }
            }
            else
            {
                foreach (var item in users)
                {
                    User user = dBContext.Users.FirstOrDefault(x => x.UserId.Equals(item));
                    if (user is null)
                    {
                        return (false, "Không tìm thấy người dùng", null);
                    }

                    EventUser eventUser = dBContext.EventUsers.FirstOrDefault(x => x.UserId.Equals(item) && x.EventId.Equals(eventId));
                    string templateStr = template.TemplateContent;

                    reviewMail.Add(new
                    {
                        Key = user.ShareholderCode,
                        Value = ReplaceTemplate(templateStr, user, events, eventUser, null)
                    });
                }
            }
                
            return (true, "Tải thư mời thành công", reviewMail);
        }
        #endregion
    }

    public class QuestionClientResponse
    {
        public string QuestionId { get; set; }
        public string EventId { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string QuestionContent { get; set; }
        public QUESTION_STATUS QuestionStatus { get; set; } 
        public bool QuestionActive { get; set; } = false;
        public DateTime QuestionCreatedAt { get; set; }

        public string QuestionComment { get; set; }
        public string QuestionUserComment { get; set; }
        public DateTime? QuestionDateComment { get; set; }
        public string UserName { get; set; }

        public QuestionClientResponse(QuestionClient questionClient)
        {
            QuestionId = questionClient.QuestionId;
            EventId = questionClient.EventId;
            SessionId = questionClient.SessionId;
            UserId = questionClient.User?.UserId;
            UserName = questionClient.User?.FullName;
            QuestionContent = questionClient.QuestionContent;
            QuestionStatus = questionClient.QuestionStatus;
            QuestionActive = questionClient.QuestionActive;
            QuestionCreatedAt = questionClient.QuestionCreatedAt;
            QuestionComment = questionClient.QuestionCommentClient.OrderByDescending(x => x.CreatedAt).FirstOrDefault()?.QuestionContent;
            QuestionUserComment = questionClient.QuestionCommentClient.OrderByDescending(x => x.CreatedAt).FirstOrDefault()?.UserName;
            QuestionDateComment = questionClient.QuestionCommentClient.OrderByDescending(x => x.CreatedAt).FirstOrDefault()?.CreatedAt;
        }
    }

}