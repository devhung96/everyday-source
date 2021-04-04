using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.Modules.Authorities.Entities;
using Project.Modules.Documents.Entities;
using Project.Modules.Events.Entities;
using Project.Modules.Events.Services;
using Project.Modules.Organizes.Entities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Organizes.Services
{
    public interface IOperatorService
    {
        public User GetInfoShareholder(string userId, string eventId);
        public Organize GetInfoOrganize(string eventId, string userId);
        public List<DocumentFile> GetDocumentOrganize(string eventId);
        public object GetSessions(string eventId);

        public ChartForEvent GetChart(string eventId, bool flagQuestion = false);
        public (bool result, string message, string code) BeginEvent(string eventId, string token);
        public (bool result, string message, string code) EndEvent(string eventId, string token);

        public int GetTotalCumulativeVotes(string questionId, string eventId);


    }
    public class OperatorService : IOperatorService
    {

        private readonly MariaDBContext _mariaDBContext;
        private readonly IConfiguration _configuration;
        private readonly string _urlMedia = "";

        private readonly string ViduURL = "";

        private readonly ISoketIO _soketIO;


        private readonly IUserSupportService _userSupportService;
        public OperatorService(MariaDBContext mariaDBContext, IConfiguration configuration, IUserSupportService userSupportService, ISoketIO soketIO)
        {
            _mariaDBContext = mariaDBContext;
            _configuration = configuration;

            _urlMedia = _configuration["MediaService:MediaUrl"];
            _userSupportService = userSupportService;
            ViduURL = configuration["OpenViduUrl"].toDefaultUrl();
            _soketIO = soketIO;
        }

        public User GetInfoShareholder(string userId , string eventId)
        {
            var user = _mariaDBContext.Users.FirstOrDefault(x => x.UserId == userId);
            if (user is null) return null;
            var userEvent = _mariaDBContext.EventUsers.FirstOrDefault(x => x.UserId == userId && x.EventId == eventId);
            user.UserStock = userEvent == null ? 0 : userEvent.UserStock;

            user.UserInviteStatus = userEvent.UserInviteStatus;

            var authorityStocks = _mariaDBContext.Authorities.Where(x => x.EventID == eventId && x.AuthorityType == Authorities.Entities.AuthorityType.EVENT && x.AuthorityReceiveUserID == userId).Sum(x => x.AuthorityShare);
            if (authorityStocks.HasValue) user.UserStock += authorityStocks.Value;

            return user;
        }

        public Organize GetInfoOrganize(string eventId , string userId)
        {
            var _event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_event is null) return null;

            var _organize = _mariaDBContext.Organizes.FirstOrDefault(x => x.OrganizeId == _event.OrganizeId);
            if (_organize is null) return null;

            _organize.EventLogoUrl = _event.EventLogoUrl.UrlCombine(_urlMedia);
            _organize.EventTimeBegin = _event.EventTimeBegin;
            _organize.EventTimeEnd = _event.EventTimeEnd;
            _organize.EventName = _event.EventName;


            // Loại bỏ những thằng đã ủy quyền ra
            var authorities = _mariaDBContext.Authorities.Where(x => x.EventID == eventId && x.AuthorityType == AuthorityType.EVENT).ToList();


            //Lấy số người tham dự  nếu chưa bắt đầu . Bắt đầu rồi thì get từ chốt. . Trừ ủy quyền ra
            if (_event.EventFlag == EVENT_FLAG.CREATED) // chua bat dau
            {
                var userEvents = _mariaDBContext.EventUsers.Where(x => x.EventId == eventId && x.UserStock > 0 && x.UserLoginStatus == USER_LOGIN_STATUS.ON &&!authorities.Select(x => x.AuthorityUserID).Contains(x.UserId)).ToList();
                _organize.NumberOfShareholders = userEvents.Count();
                _organize.EventStock = userEvents.Sum(x => x.UserStock);
                _organize.EventStock += authorities.Where(x=> userEvents.Select(x=> x.UserId).Contains(x.AuthorityReceiveUserID)).Sum(x => x.AuthorityShare.Value);
            }
            else // bat dau hoac ket thuc
            {
                var userEvents = _mariaDBContext.EventUsers.Where(x => x.EventId == eventId && x.UserLatch == USER_LATCH.ON && !authorities.Select(x => x.AuthorityUserID).Contains(x.UserId)).ToList();
                _organize.NumberOfShareholders = userEvents.Count();
                _organize.EventStock = userEvents.Sum(x => x.UserStock);
                _organize.EventStock += authorities.Where(x => userEvents.Select(x => x.UserId).Contains(x.AuthorityReceiveUserID)).Sum(x => x.AuthorityShare.Value);
            }

           


            return _organize; 
        }


        public List<DocumentFile> GetDocumentOrganize(string eventId)
        {
            var _documents = _mariaDBContext.DocumentFiles.Where(x => x.EventId == eventId).ToList();
            foreach (var item in _documents)
            {
                item.DocumentLink = item.DocumentLink.UrlCombine(_urlMedia);
            }
            return _documents;
        }

        public object GetSessions(string eventId)
        {
            var questionSession = _mariaDBContext.MiddleQuestions.ToList();
            var sessions = _mariaDBContext.Sessions.Where(x => x.EventId == eventId).OrderBy(x=>x.SessionSort).ToList();
            foreach (var item in sessions)
            {
                item.Total = questionSession.Count(x => x.SessionID == item.SessionId && x.Type == Question.Entities.TypeMiddle.ACTIVE);
            }
            return sessions;
        }

        public ChartForEvent GetChart(string eventId, bool flagQuestion = false)
        {

            var _event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_event is null) return new ChartForEvent { };

            var userEvent = _mariaDBContext.EventUsers.Where(x => x.EventId == eventId).ToList();

            // Loại bỏ những thằng đã ủy quyền ra
            var authorities = _mariaDBContext.Authorities.Where(x => x.EventID == eventId && x.AuthorityType == AuthorityType.EVENT).Select(x => x.AuthorityUserID).ToList();

            var totalNumberOfShareholders = userEvent.Count(x=> !authorities.Contains(x.UserId));
            var totalShares = userEvent.Sum(x => x.UserStock);

            if (_event.EventFlag == EVENT_FLAG.CREATED || flagQuestion)
            {
                var usreOnline = userEvent.Where(x => x.UserLoginStatus == USER_LOGIN_STATUS.ON && !authorities.Contains(x.UserId) && x.UserStock > 0).ToList();
                var totalNumberOfShareholdersAttending = usreOnline.Count();
                var totalNumberOfSharesRepresented = usreOnline.Sum(x => x.UserStock);
                totalNumberOfSharesRepresented += _mariaDBContext.Authorities.Where(x => x.EventID == eventId && x.AuthorityType == AuthorityType.EVENT && usreOnline.Select(x => x.UserId).Contains(x.AuthorityReceiveUserID)).Sum(x => x.AuthorityShare.Value);

                //prepare chart
                ChartForEvent chartForEvent = new ChartForEvent
                {
                    totalNumberOfShareholders = totalNumberOfShareholders,
                    totalNumberOfShareholdersAttending = totalNumberOfShareholdersAttending,
                    totalNumberOfSharesRepresented = totalNumberOfSharesRepresented,
                    totalShares = totalShares,
                    totalStock = totalShares
                };
                return chartForEvent;
            }
            else
            {
                var usreOnline = userEvent.Where(x => x.UserLatch == USER_LATCH.ON && !authorities.Contains(x.UserId)).ToList();

                var totalNumberOfShareholdersAttending = usreOnline.Count();
                var totalNumberOfSharesRepresented = usreOnline.Sum(x => x.UserStock);

                totalNumberOfSharesRepresented += _mariaDBContext.Authorities.Where(x => x.EventID == eventId && x.AuthorityType == AuthorityType.EVENT && usreOnline.Select(x => x.UserId).Contains(x.AuthorityReceiveUserID)).Sum(x => x.AuthorityShare.Value);

                //prepare chart
                ChartForEvent chartForEvent = new ChartForEvent
                {
                    totalNumberOfShareholders = totalNumberOfShareholders,
                    totalNumberOfShareholdersAttending = totalNumberOfShareholdersAttending,
                    totalNumberOfSharesRepresented = totalNumberOfSharesRepresented,
                    totalShares =  totalShares,
                    totalStock = totalNumberOfSharesRepresented
                };
                return chartForEvent;
            }


        }


        public (bool result , string message , string code) BeginEvent(string eventId,string token)
        {
            using (var transaction = _mariaDBContext.Database.BeginTransaction())
            {
                try
                {
                    Event _event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
                    if (_event is null) return (false, "Sự kiện không tồn tại!", "EventNotFound");

                    if (_event.EventFlag != EVENT_FLAG.CREATED) return (false, "Sự kiện đã bắt đầu hoặc kết thúc không thể được cập nhật", "EventsThatHaveAlreadyStartedOrEndedCannotBeUpdated");
                    _event.EventFlag = EVENT_FLAG.BEGIN;

                    // loai bo uy quyen
                    List<string> authorities = _mariaDBContext.Authorities.Where(x => x.EventID == eventId && x.AuthorityType == AuthorityType.EVENT).Select(x => x.AuthorityUserID).ToList();


                    List<EventUser> eventUsers = _mariaDBContext.EventUsers.Where(x => x.EventId == eventId && x.UserLoginStatus == USER_LOGIN_STATUS.ON && !authorities.Contains(x.UserId)).ToList();
                    eventUsers.ForEach(x => x.UserLatch = USER_LATCH.ON);

                    _event.EventFlag = EVENT_FLAG.BEGIN;
                    _event.EventCountDown = EVENT_COUNT_DOWN.UNALBE;
                    _mariaDBContext.SaveChanges();

                    var dataSendSocket = new
                    {
                        status = _event.EventFlag,
                        note = new
                        {
                            CREATED = 0,
                            BEGIN = 1,
                            END = 2
                        }
                    };

                    _soketIO.ForwardAsync(eventId, dataSendSocket, token, "status_event",null,"1","1");


                    // count down status

                    var result = new
                    {
                        eventId = eventId,
                        countDownStatus = _event.EventCountDown,
                        note = new
                        {
                            UNALBE = 0,
                            ALBE = 1
                        }
                    };

                    _soketIO.ForwardAsync(eventId, result, token, "status_count_down", null, "1", "1");


                    transaction.Commit();
                    return (true, "Cập nhật thành công!", "UpdateSuccess");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return (false, "Lỗi bất ngờ", $"Erro:{ex.Message}");
                }

            }
        }

        public (bool result, string message, string code) EndEvent(string eventId, string token)
        {
            Event _event = _mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_event is null) return (false, "Event not found!", "EventNotFound");
            if (_event.EventFlag == EVENT_FLAG.BEGIN)
            {
                _event.EventFlag = EVENT_FLAG.END;

                //remove all openvidu eventid

                _mariaDBContext.SaveChanges();

                var dataSendSocket = new
                {
                    status = _event.EventFlag,
                    note = new
                    {
                        CREATED = 0,
                        BEGIN = 1,
                        END = 2
                    }
                };

                _soketIO.ForwardAsync(eventId, dataSendSocket, token, "status_event", null, "1", "1");
                return (true, "Update success!", "success");
            }
            return (false, "Sự kiện đã kết thúc.", "TheEventHasNotStartedYet");
          
        }



        public int GetTotalCumulativeVotes(string questionId,string eventId)
        {
            int total = _mariaDBContext.Authorities.Where(x => x.QuestionID == questionId && x.EventID == eventId && x.AuthorityType == AuthorityType.QUESTION).Select(x=> x.AuthorityUserID).Distinct().Count();
            return total;
        }
     

    }

    
}
