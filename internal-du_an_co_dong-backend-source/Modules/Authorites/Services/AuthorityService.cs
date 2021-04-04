using Project.App.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Modules.Authorities.Entities;
using Project.Modules.Authorities.Requests;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Project.Modules.Authorites.Requests;
using System.Linq.Dynamic.Core;
using Project.App.Requests;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Project.Modules.Users.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.Events.Entities;
using Project.Modules.Events.Services;
using Project.Modules.Reports.Services;
using Project.Modules.Organizes.Services;
using Project.Modules.Question.Services;
using Project.Modules.Question.Request;
using Project.Modules.Question.Response;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Project.Modules.Authorities.Services
{
    public interface IAuthorityService
    {
        (object data, string message) StoreForVote(StoreForVoteRequest request, string token);
        Authority StoreForAuthority(StoreForAuthority request, string token);
        object ShowAllInEvent(RequestTable table, string eventID, string questionID);
        (Authority data, string message) Show(int authorityID);
        object ListUserAuthority(string userReceiveID, string eventID, string questionID);
        void DeleteFromEvent(string userID, string eventID);
        (object data, string message) ChangeStatus(int authorityID, ChangeStatus request);
        List<Authority> ShowAll();
        List<Authority> ShowAllVote(string eventID);
        object ShowAllAuthorized(string eventID, RequestTable table);
        Authority UpdateAuthorityWithID(UpdateForAuthorityWithID request);
        (object data, string message) GetUser(string shareHolderCode, string eventID);
        List<User> GetListUser(string eventID, string shareHolderCode);
        (Authority data, string message) UpdateAuthority(UpdateAuthority request);
        List<User> GetListUserVote(string eventID, string questionID);
        (bool flag, string message) DeleteAuthority(int authorityID, string token);
        string ResetVote(string eventID, string questionID);
    }
    public class AuthorityService : IAuthorityService
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly MongoDBContext mongoDBContext;
        private readonly ISoketIO soketIO;
        private readonly IReportService reportService;
        private readonly IOperatorService operatorService;
        private readonly IConfiguration configuration;
        //private readonly IQuestionAnswersService questionAnswersService;
        public AuthorityService(MariaDBContext mariaDBContext, MongoDBContext mongoDBContext, ISoketIO soketIO, IReportService reportService, IOperatorService operatorService, IConfiguration configuration)
        {
            this.mariaDBContext = mariaDBContext;
            this.mongoDBContext = mongoDBContext;
            this.soketIO = soketIO;
            this.reportService = reportService;
            this.operatorService = operatorService;
            this.configuration = configuration;
        }
        //Support Pagination
        public string OrderValue(string sortColumn, string SortDir)
        {
            return sortColumn + " " + SortDir;
        }
        public (object data, string message) StoreForVote(StoreForVoteRequest request, string token)
        {
            try
            {
                if (request.Errors.Count > 0) // check validate
                    return (request.Errors, "VoteError");
                else
                {
                    List<Authority> authorities = new List<Authority>();
                    foreach (var item in request.VoteRequests)
                    {
                        Authority authority = new Authority()
                        {
                            EventID = request.EventID,
                            AuthorityReceiveUserID = item.UserReceiveID,
                            AuthorityUserID = request.UserID,
                            AuthorityType = AuthorityType.QUESTION,
                            AuthorityStatus = AuthorityStatus.CONFIRM,
                            AuthorityShare = item.Share,
                            QuestionID = request.QuestionID
                        };
                        mariaDBContext.Authorities.Add(authority);
                        authority.AuthorityReceiveUserInfo = mariaDBContext.Users.Find(authority.AuthorityReceiveUserID);
                        authority.AuthorityUserInfo = mariaDBContext.Users.Find(authority.AuthorityUserID);
                        authorities.Add(authority);
                        mariaDBContext.SaveChanges();
                    }
                    #region realtime chart 

                    string member = reportService.GetOptionQuestion(request.QuestionID);
                    var dataSendSocket = reportService.GeneratorDataChartResponse(request.EventID, request.QuestionID, true, "displayShowChart(Hiện thị chart khi màn hình vận hành ấn):  true = show , false = hidden");
                    soketIO.ForwardAsync(request.EventID, dataSendSocket, token, "realtimeChart", null, "1", member);

                    #endregion

                    #region Gọi socket
                    var listSendSoketVote = mariaDBContext.Authorities
                        .Where(x => x.EventID.Equals(request.EventID) && x.QuestionID.Equals(request.QuestionID) && x.AuthorityUserID.Equals(request.UserID))
                        .ToList();
                    //cổ phần tính tổng những người bầu dồn cho user nhận
                    listSendSoketVote.ForEach(a => a.AuthorityShare = mariaDBContext.Authorities
                    .Where(x => x.EventID.Equals(request.EventID) && x.QuestionID.Equals(request.QuestionID) && x.AuthorityReceiveUserID.Equals(a.AuthorityReceiveUserID))
                    .Sum(x => x.AuthorityShare));
                    var listMember = request.VoteRequests.Select(x => x.UserReceiveID).ToList();
                    soketIO.ForwardAsync(request.EventID, listSendSoketVote, token, "listvoted", listMember, "0");
                    #endregion
                    return (null, "CreateVoteSuccess");
                }
            }
            catch (Exception)
            {
                request.Errors.Add(new { UserReceiveID = "", Share = 0, MessageError = "YouHaveCastVotesOnAnotherDevice" });
                return (request.Errors, "VoteError");
                //return (null, ex.InnerException.Message);
            }
            
        }
        public Authority StoreForAuthority(StoreForAuthority request, string token)
        {
            Authority authority = new Authority()
            {
                EventID = request.EventID,
                AuthorityReceiveUserID = request.UserReceiveID,
                AuthorityUserID = request.UserID,
                AuthorityType = AuthorityType.EVENT,
                AuthorityStatus = AuthorityStatus.CONFIRM,
                AuthorityShare = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(request.EventID) && x.UserId.Equals(request.UserID)).UserStock
            };
            mariaDBContext.Authorities.Add(authority);
            mariaDBContext.SaveChanges();
            authority.AuthorityReceiveUserInfo = mariaDBContext.Users.Find(authority.AuthorityReceiveUserID);
            authority.AuthorityUserInfo = mariaDBContext.Users.Find(authority.AuthorityUserID);
            // send socket 
            ChartForEvent dataSendSocket = operatorService.GetChart(request.EventID);
            soketIO.ForwardAsync(request.EventID, dataSendSocket, token, "event_statistics", null, "1", "0");

            return authority;
        }
        public object ShowAllInEvent(RequestTable table, string eventID, string questionID)
        {
            List<Authority> authorities = mariaDBContext.Authorities
                .Where(x => x.EventID.Equals(eventID) && x.QuestionID.Equals(questionID) && x.AuthorityType == AuthorityType.QUESTION)
                .ToList();
            foreach (Authority authority in authorities)
            {
                //set stock cho user nhận
                mariaDBContext.Users.Find(authority.AuthorityReceiveUserID).UserStock = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(authority.EventID) && x.UserId.Equals(authority.AuthorityReceiveUserID)).UserStock;
                // set stock cho user cho
                mariaDBContext.Users.Find(authority.AuthorityUserID).UserStock = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(authority.EventID) && x.UserId.Equals(authority.AuthorityUserID)).UserStock;
                authority.AuthorityReceiveUserInfo = mariaDBContext.Users.Find(authority.AuthorityReceiveUserID);
                authority.AuthorityUserInfo = mariaDBContext.Users.Find(authority.AuthorityUserID);
            }
            if (table.Type == 1)
            {
                var result = authorities.AsQueryable().OrderBy(OrderValue(table.SortField, table.SortOrder)).ToList();
                ResponseTable responseTable = new ResponseTable()
                {
                    DateResult = result.Skip((table.Page - 1) * table.Results).Take(table.Results).ToList(),
                    Info = new Info()
                    {
                        Page = table.Page,
                        TotalRecord = result.Count,
                        Results = table.Results
                    }
                };
                return responseTable;
            }
            else
            {
                object resultNoPagiation = new
                {
                    results = authorities,
                    info = new
                    {
                        page = 0,
                        totalRecord = 0,
                        results = 0
                    }
                };
                return (resultNoPagiation);
            }
        }
        public (Authority data, string message) Show(int authorityID)
        {
            Authority authority = mariaDBContext.Authorities.Find(authorityID);
            if (authority is null)
                return (null, "AuthorizationNotFound");
            authority.AuthorityReceiveUserInfo = mariaDBContext.Users.Find(authority.AuthorityReceiveUserID);
            authority.AuthorityUserInfo = mariaDBContext.Users.Find(authority.AuthorityUserID);
            return (authority, null);
        }
        public object ListUserAuthority(string userReceiveID, string eventID, string questionID)
        {
            List<Authority> authorities = mariaDBContext.Authorities
                .Where(x => x.AuthorityReceiveUserID.Equals(userReceiveID) && x.EventID.Equals(eventID) && x.QuestionID.Equals(questionID))
                .ToList();
            foreach (Authority authority in authorities)
            {
                authority.AuthorityReceiveUserInfo = mariaDBContext.Users.Find(userReceiveID);
                authority.AuthorityUserInfo = mariaDBContext.Users.Find(authority.AuthorityUserID);
            }
            return authorities;
        }

        public void DeleteFromEvent(string userID, string eventID)
        {
            List<Authority> authorities = mariaDBContext.Authorities
                 .Where(x => x.EventID.Equals(eventID) && (x.AuthorityUserID.Equals(userID) || x.AuthorityReceiveUserID.Equals(userID)))
                 .ToList();
            mariaDBContext.Authorities.RemoveRange(authorities);
            mariaDBContext.SaveChanges();
        }

        public (object data, string message) ChangeStatus(int authorityID, ChangeStatus request)
        {
            Authority authority = mariaDBContext.Authorities.Find(authorityID);
            if (authority is null)
                return (null, "AuthorityNotFound");
            authority.AuthorityStatus = request.AuthorityStatus;
            mariaDBContext.SaveChanges();
            return (authority, null);
        }

        public List<Authority> ShowAll()
        {
            List<Authority> authorities = mariaDBContext.Authorities.ToList();
            foreach (Authority authority in authorities)
            {
                authority.AuthorityReceiveUserInfo = mariaDBContext.Users.Find(authority.AuthorityReceiveUserID);
                authority.AuthorityUserInfo = mariaDBContext.Users.Find(authority.AuthorityUserID);
            }
            return authorities;
        }
        public object ShowAllAuthorized(string eventID, RequestTable table) //trả về tất cả ủy quyền tại một event
        {
            List<Authority> authorities = mariaDBContext.Authorities
                .Where(x => x.EventID.Equals(eventID) && x.AuthorityType == AuthorityType.EVENT)
                .ToList();
            foreach (Authority authority in authorities)
            {
                string groupNameUser = "";
                string groupNameUserReceive = "";
                int groupIDUser = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(authority.EventID) && x.UserId.Equals(authority.AuthorityUserID)).GroupId;
                int groupIDUserReceive = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(authority.EventID) && x.UserId.Equals(authority.AuthorityReceiveUserID)).GroupId;
                var groupNameByUser = mariaDBContext.Groups.FirstOrDefault(x => x.GroupID == groupIDUser);
                var groupNameByUserReceive = mariaDBContext.Groups.FirstOrDefault(x => x.GroupID == groupIDUserReceive);
                if (groupNameByUser != null)
                {
                    groupNameUser = groupNameByUser.GroupName;
                }
                if (groupNameByUserReceive != null)
                {
                    groupNameUserReceive = groupNameByUserReceive.GroupName;
                }
                //set stock cho user nhận
                mariaDBContext.Users.Find(authority.AuthorityReceiveUserID).UserStock = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(authority.EventID) && x.UserId.Equals(authority.AuthorityReceiveUserID)).UserStock;
                // set stock cho user cho
                mariaDBContext.Users.Find(authority.AuthorityUserID).UserStock = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(authority.EventID) && x.UserId.Equals(authority.AuthorityUserID)).UserStock;
                authority.AuthorityReceiveUserInfo = new { user = mariaDBContext.Users.Find(authority.AuthorityReceiveUserID), role = groupNameUserReceive };
                authority.AuthorityUserInfo = new { user = mariaDBContext.Users.Find(authority.AuthorityUserID), role = groupNameUser };
            }
            if(table.Type == 1)
            {
                var result = authorities.AsQueryable().OrderBy(OrderValue(table.SortField, table.SortOrder)).ToList();
                ResponseTable responseTable = new ResponseTable()
                {
                    DateResult = result.Skip((table.Page - 1) * table.Results).Take(table.Results).ToList(),
                    Info = new Info()
                    {
                        Page = table.Page,
                        TotalRecord = result.Count,
                        Results = table.Results
                    }
                };
                return responseTable;
            }
            else
            {
                object resultNoPagiation = new
                {
                    results = authorities,
                    info = new
                    {
                        page = 0,
                        totalRecord = 0,
                        results = 0
                    }
                };
                return (resultNoPagiation);
            }
        }
        public List<Authority> ShowAllVote(string eventID) //trả về tất cả bầu dồn phiếu tại một event
        {
            List<Authority> authorities = mariaDBContext.Authorities
                .Where(x => x.EventID.Equals(eventID) && x.AuthorityType == AuthorityType.QUESTION)
                .ToList();
            foreach (Authority authority in authorities)
            {
                authority.AuthorityReceiveUserInfo = mariaDBContext.Users.Find(authority.AuthorityReceiveUserID);
                authority.AuthorityUserInfo = mariaDBContext.Users.Find(authority.AuthorityUserID);
            }
            return authorities;
        }
        public Authority UpdateAuthorityWithID(UpdateForAuthorityWithID request)
        {
            Authority authority = mariaDBContext.Authorities.Find(request.AuthorityID.Value);
            authority.AuthorityReceiveUserID = request.UserReceive;
            authority.AuthorityUpdateAt = DateTime.Now;
            mariaDBContext.SaveChanges();
            authority.AuthorityReceiveUserInfo = mariaDBContext.Users.Find(authority.AuthorityReceiveUserID);
            authority.AuthorityUserInfo = mariaDBContext.Users.Find(authority.AuthorityUserID);
            return authority;
        }
        public (object data, string message) GetUser(string shareHolderCode, string eventID)
        {
            List<string> users = mariaDBContext.Users.Where(x => x.ShareholderCode.Equals(shareHolderCode)).Select(x => x.UserId).ToList();
            if (users.Count == 0)
                return (null, "UserNotFound");
            EventUser eventUser = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(eventID) && users.Contains(x.UserId));
            if (eventUser is null)
                return (null, "UserDoesNotExistInThisEvent");
            User user = mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(eventUser.UserId));
            if (user is null)
                return (null, "UserNotFond");
            var result = new
            {
                user = user,
                userStock = eventUser.UserStock
            };
            return (result, "GetUserInformationSuccessfully");
        }

        public List<User> GetListUser(string eventID, string shareHolderCode)
        {
            var userstemp = mariaDBContext.Users.Where(x => x.ShareholderCode.Equals(shareHolderCode)).Select(x => x.UserId).ToList();
            string userId = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(eventID) && userstemp.Contains(x.UserId)).UserId;
            List<string> userAuthorized = mariaDBContext.Authorities.Where(x => x.EventID.Equals(eventID)).Select(x => x.AuthorityUserID).ToList(); // filter những user đã ủy quyền    
            List<string> userID = mariaDBContext.EventUsers
                .Where(x => x.EventId.Equals(eventID) && !userAuthorized.Contains(x.UserId) && !x.UserId.Equals(userId))
                .Select(x => x.UserId)
                .ToList();
            if (userID.Count == 0)
                return null;
            List<User> users = mariaDBContext.Users.Where(x => userID.Contains(x.UserId)).ToList();
            return users;
        }
        public List<User> GetListUserVote(string eventID,string questionID)
        {
            Event @event = mariaDBContext.Events.Find(eventID);
            List<User> users = new List<User>();
            if (@event is null)
                return users;
            GetResultSubmit getResultSubmit = new GetResultSubmit();
            getResultSubmit.QuestionID = questionID;
            var result = GetResultSubmitVoid(getResultSubmit);
            List<string> userIDs = new List<string>();
            if (result.result != null)
            {
                var ListcheckAnswered = (List<ResponseResultSubmit>)result.result;
                userIDs = ListcheckAnswered.Select(x => x.ContentJson.UserID).ToList(); // lấy danh sách những ng đã trả lời câu hỏi đó
            }
            if (@event.EventFlag == EVENT_FLAG.CREATED)
            {
                #region Get list user have stock = 0 (userStock + authority)
                List<string> userReceiveAuthority = mariaDBContext.Authorities
                    .Where(x => x.EventID.Equals(eventID) && x.AuthorityType == AuthorityType.EVENT)
                    .Select(x => x.AuthorityReceiveUserID)
                    .ToList();
                List<string> userIDEvent = mariaDBContext.EventUsers
                    .Where(x => x.EventId.Equals(eventID) && x.UserLoginStatus == USER_LOGIN_STATUS.ON && x.UserStock == 0 && !userReceiveAuthority.Contains(x.UserId))
                    .Select(x => x.UserId)
                    .ToList();
                #endregion
                List<string> userVoted = mariaDBContext.Authorities
                    .Where(x => x.EventID.Equals(eventID) && (x.QuestionID.Equals(questionID)) || x.AuthorityType == AuthorityType.EVENT)
                    .Select(x => x.AuthorityUserID)
                    .ToList();
                List<string> userID = mariaDBContext.EventUsers
                    .Where(x => x.EventId.Equals(eventID) && !userVoted.Contains(x.UserId) && !userIDs.Contains(x.UserId) && !userIDEvent.Contains(x.UserId) && x.UserLoginStatus == USER_LOGIN_STATUS.ON)
                    .Select(x => x.UserId)
                    .ToList();
                if (userID.Count == 0)
                    return users;
                users = mariaDBContext.Users.Where(x => userID.Contains(x.UserId)).ToList();
                return users;
            }
            else
            {
                List<string> userVoted = mariaDBContext.Authorities
                    .Where(x => x.EventID.Equals(eventID) && (x.QuestionID.Equals(questionID)) || x.AuthorityType == AuthorityType.EVENT)
                    .Select(x => x.AuthorityUserID)
                    .ToList();
                List<string> userID = mariaDBContext.EventUsers
                    .Where(x => x.EventId.Equals(eventID) && !userVoted.Contains(x.UserId) && !userIDs.Contains(x.UserId) && x.UserLatch == USER_LATCH.ON)
                    .Select(x => x.UserId)
                    .ToList();
                if (userID.Count == 0)
                    return users;
                users = mariaDBContext.Users.Where(x => userID.Contains(x.UserId)).ToList();
                return users;
            }
        }

        public (Authority data, string message) UpdateAuthority(UpdateAuthority request)
        {
            Authority authority = mariaDBContext.Authorities.FirstOrDefault(x =>
            x.EventID.Equals(request.EventID)
            && x.AuthorityUserID.Equals(request.UserID)
            && x.AuthorityType == AuthorityType.EVENT);
            #region check
            Event _event = mariaDBContext.Events.FirstOrDefault(x => x.EventId.Equals(authority.EventID) && x.EventFlag != EVENT_FLAG.END);
            if (_event is null)
                return (null, "TheEventHasEnded");
            var checkEventUser = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId.Equals(authority.EventID) && x.UserId.Equals(request.UserReceiveID));
            if (checkEventUser is null)
                return (null, "UserDoesNotExistInThisEvent");
            var checkAuthorityEvent = mariaDBContext.Authorities.FirstOrDefault(x => x.EventID.Equals(authority.EventID) && x.AuthorityUserID.Equals(request.UserReceiveID));
            if (checkAuthorityEvent != null)
                return (null, "ThisUserHasThePermissionsForOtherPeopleInThisEvent");
            #endregion

            authority.AuthorityReceiveUserID = request.UserReceiveID;
            mariaDBContext.SaveChanges();
            authority.AuthorityReceiveUserInfo = mariaDBContext.Users.Find(authority.AuthorityReceiveUserID);
            authority.AuthorityUserInfo = mariaDBContext.Users.Find(authority.AuthorityUserID);
            return (authority, "UpdateAuthoritySuccess");
        }
        public (bool flag , string message) DeleteAuthority(int authorityID, string token)
        {
            Authority authority = mariaDBContext.Authorities.Find(authorityID);
            if (authority is null)
                return (false, "AuthorizationNotFound");
            Event _event = mariaDBContext.Events.FirstOrDefault(x => x.EventId.Equals(authority.EventID));
            if (_event is null)
            {
                return (false, "EventNotFound");
            }
            if(_event.EventFlag == EVENT_FLAG.BEGIN)
            {
                return (false, "TheEventHasBegun");
            }
            mariaDBContext.Remove(authority);
            mariaDBContext.SaveChanges();
            // send socket 
            ChartForEvent dataSendSocket = operatorService.GetChart(_event.EventId);
            soketIO.ForwardAsync(_event.EventId, dataSendSocket, token, "event_statistics", null, "1", "0");
            return (true, "AuthorizationDeletedSuccessfully");
        }

        public string ResetVote(string eventID, string questionID )
        {
            List<Authority> authorities = mariaDBContext.Authorities
                .Where(x => x.EventID.Equals(eventID) && x.QuestionID.Equals(questionID) && x.AuthorityType == AuthorityType.QUESTION)
                .ToList();
            if(authorities != null)
            {
                mariaDBContext.RemoveRange(authorities);
                mariaDBContext.SaveChanges();
            }
            return "ResetWasSuccessful";
        }
        private (object result, string message) GetResultSubmitVoid(GetResultSubmit getResultSubmit)
        {
            getResultSubmit.AppId = configuration["SurveyQuestion:AppId"];
            (string data, int? statusCode) resultSubmit = HttpMethod.Get.SendRequestWithStringContentAsync(configuration["SurveyQuestion:Url"] + $"/submit/get-all/{getResultSubmit.AppId}", null, null).Result;

            if (resultSubmit.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultSubmit.data);
            }

            var responseSubmit = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultSubmit.data);

            var listResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseResultSubmit>>(responseSubmit.Data.ToString());
            if (listResult.Count == 0)
            {
                return (null, "TheQuestionHasNoResults");
            }
            var response = listResult.Where(x => x.QuestionID == getResultSubmit.QuestionID).ToList();
            return (response, "Success.");
        }
    }
}
