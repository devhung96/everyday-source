using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project.App.Database;
using Project.Modules.Authorities.Entities;
using Project.Modules.Documents.Entities;
using Project.Modules.Events.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Invitations.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.Organizes.Services;
using Project.Modules.PermissionGroups.Entities;
using Project.Modules.Question.Entities;
using Project.Modules.Sessions.Entities;
using Project.Modules.UserPermissionEvents.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Services
{
    public interface IEventService
    {
        public (Event Event, string message, string code) Create(Event newEvent, string url = "");
        public (Event Event, string message, string code) Update(string IdEvent, Event newEvent, string url = "");
        public (bool result, string message, string code) Delete(string IdEvent);
        public (Event Event, string message, string code) Show(string IdEvent, string url= "");
        public (List<Event> Events, string message, string code) ShowAll(string url = "");
        public (List<Event> Events, string message, string code) ShowEventByOrganize(string idOrganize, string url = "");
        public (object result, string message, string code) CheckIn(string userId, string eventId);
        public (object result, string message, string code) CheckOut(string userId, string eventId);
        public (bool result, string message, string code) CheckOutAll();
        public (Event result, bool status, string message, string code) CheckEvent(string eventId);

        public (object result, string message, string messageCode) UpdateCountDown(string eventId, EVENT_COUNT_DOWN status, string token);

        public (Event data, string message, string code) UpdateSetting(string IdEvent, string eventSetting);

    }
    public class EventService: IEventService
    {
        private readonly IConfiguration _configuration;
        private readonly MariaDBContext _mariaDBContext;

        private readonly IEventUserService _eventUserService;
        private readonly ISoketIO _soketIO;
        private readonly string _urlMedia = "";



        public EventService(MariaDBContext mariaDBContext, IConfiguration configuration, IEventUserService eventUserService, ISoketIO soketIO)
        {
            _mariaDBContext = mariaDBContext;
            _configuration = configuration;
            _eventUserService = eventUserService;
            _soketIO = soketIO;
            _urlMedia = _configuration["MediaService:MediaUrl"];

        }

        public (Event Event, string message, string code) Create(Event newEvent, string url = "")
        {
            Event Event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == newEvent.EventId);
            if (Event != null) return (null, "Sự kiện đã tồn tại rồi", "TheEventAlreadyExists");
            Organize organize = _mariaDBContext.Organizes.FirstOrDefault(x=>x.OrganizeId == newEvent.OrganizeId);
            if(organize != null)
            {
                var setting = new
                 {
                    landingPage = organize.OrganizeLandingPageUrl,
                    timeQuestion = 0
                };
                newEvent.EventSetting = JsonConvert.SerializeObject(setting);

            }

            _mariaDBContext.Events.Add(newEvent);
            _mariaDBContext.SaveChanges();
            newEvent.EventLogoUrl = newEvent.EventLogoUrl.UrlCombine(url);
            return (newEvent, "Tạo sự kiện thành công", "CreatedEventSuccess");
        }


        public (Event Event, string message, string code) Update(string IdEvent, Event newEvent, string url = "")
        {
            (Event checkEvent, string checkMessage, string checkCode) = this.Show(IdEvent);
            if (checkEvent is null) return (checkEvent, checkMessage, checkCode);
            newEvent.EventId = IdEvent;

            // check time 
            if(checkEvent.EventTimeBegin.ToString("yyyy-MM-dd HH:mm") != newEvent.EventTimeBegin.ToString("yyyy-MM-dd HH:mm"))
            {
                var currentValue = newEvent.EventTimeBegin.ToString("yyyy-MM-dd HH:mm");
                var dateTimeNow = DateTime.UtcNow.AddHours(7).ToString("yyyy-MM-dd HH:mm");

                if (String.Compare(dateTimeNow, currentValue) >= 0)
                    return (null, "Thời gian bắt đầu lớn hơn thời gian hiện tại", "TheStartTimeIsGreaterThanTheCurrentTime");
            }

            //if (checkEvent.EventFlag == EVENT_FLAG.END) return (null, "Sự kiện đã kêt thúc không thể cập nhật thông tin", "TheCompletedEventCouldNotBeUpdated");

            var _newEvent = GeneralHelper.CheckUpdateObject<Event>(checkEvent, newEvent);
            _newEvent.UpdatedAt = DateTime.UtcNow.AddHours(7);
            _mariaDBContext.Entry(checkEvent).CurrentValues.SetValues(_newEvent);
            _mariaDBContext.SaveChanges();
            _newEvent.EventLogoUrl = _newEvent.EventLogoUrl.UrlCombine(url);
            return (_newEvent,"Cập nhật sự kiện thành công", "UpdateEventSuccessfully");
        }


        public (Event data, string message, string code) UpdateSetting(string IdEvent,  string eventSetting)
        {
            (Event checkEvent, string checkMessage, string checkCode) = this.Show(IdEvent);
            if (checkEvent is null) return (checkEvent, checkMessage, checkCode);
            checkEvent.EventSetting = eventSetting;
            _mariaDBContext.SaveChanges();
            return (checkEvent, "Cập nhật sự kiện thành công", "UpdateEventSuccessfully");
        }

        public (bool result, string message, string code) Delete(string IdEvent)
        {
            (Event checkEvent, string checkMessage, string checkCode) = this.Show(IdEvent);
            if (checkEvent is null) return (false, checkMessage, checkCode);
            if (checkEvent.EventFlag == EVENT_FLAG.BEGIN) return (false, "Sự kiện đã bắt đầu Không thể xóa.", "UnableToDeleteEvent");

            using (var transaction = _mariaDBContext.Database.BeginTransaction())
            {
                try
                {
                    // shareholder_authority
                    List<Authority> authorites = _mariaDBContext.Authorities.Where(x => x.EventID == checkEvent.EventId).ToList();
                    if (authorites != null)
                    {
                        _mariaDBContext.Authorities.RemoveRange(authorites);
                    }


                    //template
                    List<Template> templates = _mariaDBContext.Templates.Where(x => x.EventId == checkEvent.EventId).ToList();
                    _mariaDBContext.Templates.RemoveRange(templates);

                    // thu moi
                    List<Invitation> invitations = _mariaDBContext.Invitations.Where(x => x.EventId == checkEvent.EventId).ToList();
                    _mariaDBContext.Invitations.RemoveRange(invitations);


                    //
                    List<QuestionClient> questionClients = _mariaDBContext.QuestionClients.Where(x => x.EventId == checkEvent.EventId).ToList();
                    // command 
                    List<QuestionCommentClient> questionCommentClients = _mariaDBContext.QuestionCommentClients.Where(x => questionClients.Select(x => x.QuestionId).Contains(x.QuestionClientId)).ToList();
                    _mariaDBContext.QuestionCommentClients.RemoveRange(questionCommentClients);
                    _mariaDBContext.QuestionClients.RemoveRange(questionClients);
                    _mariaDBContext.SaveChanges();



                    // permisson group
                    List<UserPermissionEvent> userPermissionEvents = _mariaDBContext.UserPermissionEvents.Where(m => m.EventId.Equals(checkEvent.EventId)).ToList();
                    _mariaDBContext.RemoveRange(userPermissionEvents);

                    // group
                    List<Group> groups = _mariaDBContext.Groups.Where(m => m.EventId.Equals(checkEvent.EventId)).ToList();
                    List<int> groupIds = groups.Select(x => x.GroupID).ToList();
                    List<PermissionGroup> permissionGroups = _mariaDBContext.PermissionGroups.Where(m => groupIds.Equals(m.GroupId)).ToList();

                    _mariaDBContext.PermissionGroups.RemoveRange(permissionGroups);
                    _mariaDBContext.Groups.RemoveRange(groups);

                    //user in event
                    List<EventUser> eventUsers = _mariaDBContext.EventUsers.Where(m => m.EventId.Equals(checkEvent.EventId)).ToList();
                    _mariaDBContext.EventUsers.RemoveRange(eventUsers);


                    //DocumentFile
                    List<DocumentFile> documentFiles = _mariaDBContext.DocumentFiles.Where(x => x.EventId == checkEvent.EventId).ToList();
                    _mariaDBContext.RemoveRange(documentFiles);


                    // secction 
                    List<Session> sessions = _mariaDBContext.Sessions.Where(x => x.EventId == checkEvent.EventId).ToList();
                    _mariaDBContext.Sessions.RemoveRange(sessions);


                    List<MiddleQuestion> middleQuestions = _mariaDBContext.MiddleQuestions.Where(x => sessions.Select(y => y.SessionId).Contains(x.SessionID)).ToList();
                    _mariaDBContext.MiddleQuestions.RemoveRange(middleQuestions);

                    _mariaDBContext.SaveChanges();

                    // chot
                    checkEvent.EventStatus = EVENT_STATUS.DELETED;
                    _mariaDBContext.SaveChanges();

                    transaction.Commit();
                    return (true, $"Xóa sự kiện thành công!", "DeletedEventSuccess");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                    return (false, "Lỗi bất ngờ", "Erro");
                }

            }
                
        }


        public (Event Event, string message, string code) Show(string IdEvent, string url = "")
        {
            Event Event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == IdEvent);
            if (Event is null) return (null, "Không tìm thấy sự kiện!", "EventNotFound");
            if (!String.IsNullOrEmpty(url))
            {
                Event.EventLogoUrl = Event.EventLogoUrl.UrlCombine(url);  
            }

            return (Event, "Hiển thị thông tin thành công!", "GetEventSuccess");
        }


        public (List<Event> Events, string message, string code) ShowAll(string url = "")
        {
            List<Event> data = _mariaDBContext.Events.ToList();
            if (!String.IsNullOrEmpty(url))
            {
                foreach (var item in data)
                {
                    item.EventLogoUrl = item.EventLogoUrl.UrlCombine(url);
                }
            }
            return (data, "Hiển thị thành công!", "ShowAllSuccess");
        }

        public (List<Event> Events, string message, string code) ShowEventByOrganize(string idOrganize, string url = "")
        {
            List<Event> data = _mariaDBContext.Events.Where(x => x.OrganizeId == idOrganize).ToList();

            if (!String.IsNullOrEmpty(url))
            {
                foreach (var item in data)
                {
                    item.EventLogoUrl = item.EventLogoUrl.UrlCombine(url);
                }
            }
            return (data, "Hiển thị thành công", "ShowAllSuccess");
        }


        public(object result, string message, string code) CheckOut(string userId, string eventId)
        {
            EventUser eventUser = _mariaDBContext.EventUsers.FirstOrDefault(x => x.UserId == userId && x.EventId == eventId);
            if (eventUser is null) return (null, "Thông tin không đúng", "IncorrectInformation");
            eventUser.UserLoginStatus = USER_LOGIN_STATUS.OFF;
            _mariaDBContext.SaveChanges();
            return (new { result = true }, "Đăng xuất thành công!", "LogoutSuccess");
        }

        public (bool result , string message, string code) CheckOutAll()
        {
            List<EventUser> eventUsers = _mariaDBContext.EventUsers.ToList();
            eventUsers.ForEach(x => x.UserLoginStatus = USER_LOGIN_STATUS.OFF);
            _mariaDBContext.SaveChanges();
            return (true, "Đăng xuất thành công", "LogoutSuccess");
        }

        public (object result, string message, string code) CheckIn(string userId, string eventId)
        {
            Event eventPrepare = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);

            if (eventPrepare is null) return (null, "Không tìm thấy sự kiện", "EventNotFound");

            EventUser eventUser = _mariaDBContext.EventUsers.FirstOrDefault(x => x.UserId == userId && x.EventId == eventId);
            if (eventUser is null) return (null, "Thông tin không đúng", "IncorrectInformation");

            if (eventPrepare.EventFlag == EVENT_FLAG.END) return (null, "Sự kiện đã kết thúc", "TheEventHasEnded");

            if (eventPrepare.EventFlag == EVENT_FLAG.BEGIN && eventUser.UserLatch == USER_LATCH.OFF) return (null, "Sự kiện đã diễn ra không có quyền truy cập vào", "");

            eventUser.UserLoginStatus = USER_LOGIN_STATUS.ON;
            _mariaDBContext.SaveChanges();
            return (new { result = true }, "Đăng nhập thành công!", "LogintSuccess");
        }

        public (Event result,bool status, string message, string code) CheckEvent(string eventId)
        {
            Event _event = _mariaDBContext.Events.Find(eventId);
            if(_event is null)
            {

                return (null,false, "Sự kiện không tồn tại.","EventNotFound");
            
            }

            _event.EventLogoUrl= _event.EventLogoUrl.UrlCombine(_urlMedia);
           
            return (_event,true,"Sự kiện có tồn tại.","EventExists" );
        }


        public (object result, string message, string messageCode) UpdateCountDown(string eventId, EVENT_COUNT_DOWN status, string token)
        {
            Event _event = _mariaDBContext.Events.FirstOrDefault(x=> x.EventId == eventId);
            if (_event is null) return (null, "Không tìm thấy event", "EventNotFound");

            _event.EventCountDown = status;
            _mariaDBContext.SaveChanges();

            var result = new
            {
                eventId = eventId,
                countDownStatus = status,
                note = new
                {
                    UNALBE = 0,
                    ALBE = 1
                }
            };

            _soketIO.ForwardAsync(eventId, result, token, "status_count_down", null, "1", "1");
            return (result, "Cập nhật thành công!", "UpdateSuccess");
        }
    }
}
