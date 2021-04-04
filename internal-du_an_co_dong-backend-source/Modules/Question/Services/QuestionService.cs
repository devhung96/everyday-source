using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Services.Common;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Project.App.Database;
using Project.Modules.Authorities.Services;
using Project.Modules.Question.Entities;
using Project.Modules.Question.Request;
using Project.Modules.Question.Response;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project.Modules.Question.Services
{
    public interface IQuestionAnswersService
    {
        (object result, string message) StoreQuestion(NewQuestion newQuestion);
        (object result, string message) UpdateQuestion(UpdateQuestion updateQuestion, string QuestionID);
        (object result, string message) DeleteQuestion(DeleteRequest delete);
        (object result, string message) ShowAll();
        (List<ResponseQuestion> result, string message) ShowAll(string SessionID);
        (object result, string message) GetResultSubmitVoid(GetResultSubmit getResultSubmit);
        (object result, string message) GetQuestionByList(GetQuestionByList getQuestionByList);
        (object result, string message) UpdatePositon(UpdatePosition updatePosition);
        (object result, string message) ChangeIsSent(IsSent isSent);
        (object result, string message) ResetQuestion(DeleteSubmitOnePerson resetQuestion);
        (List<ResponseResultSubmitQuestion> result, string message) GetResultSubmitQuestionId(GetResultSubmit getResultSubmit);
        (object result, string message) DeleteMultiSubmitOnePerson(DeleteSubmitMulti deleteSubmitMulti);
        (List<ResponseQuestion> result, string message) ShowAllQuestionWithEvent(string EventId);
        (object result, string message) FlagQuestion(UpdateBeforeEvent updateBeforeEvent);
        (List<ResponseResultSubmit> result, string message) GetResultSubmitDuplicate(GetResultSubmitDuplicate getResultSubmitDuplicate);
    }
    public class QuestionAnswersService : IQuestionAnswersService
    {
        public readonly MariaDBContext _dbContext;
        public readonly IConfiguration _config;
        public readonly IAuthorityService _author;
        public QuestionAnswersService(MariaDBContext dbContext, IConfiguration config, IAuthorityService author)
        {
            _dbContext = dbContext;
            _config = config;
            _author = author;
        }
        public (object result, string message) StoreQuestion(NewQuestion newQuestion)
        {
            #region Check Count Answers
            if (newQuestion.CountAnswers != null && newQuestion.Type == TypeQuestion.MULTICHOOSE && newQuestion.Answers != null)
            {
                int feedback = 0;
                if (newQuestion.Feedback != null)
                {
                    feedback++;
                }
                if (newQuestion.CountAnswers > (newQuestion.Answers.Count + feedback))
                {
                    return (null, "SelectedError");
                }
                int bingo = 0;
                foreach (var item in newQuestion.Answers)
                {
                    if (item.Bingo == true)
                    {
                        bingo++;
                    }
                }
                if (newQuestion.Feedback != null && newQuestion.Feedback.Bingo == true)
                {
                    bingo++;
                }
                if (newQuestion.CountAnswers < bingo)
                {
                    return (null, "SelectedDefaultError");
                }
            }
            #endregion

            #region Custome URI

            #region MEDIA
            var listMedia = newQuestion.Media;
            if (listMedia.Count > 0)
            {
                int i = 0;
                foreach (var item in listMedia)
                {
                    #region Check URL
                    string localPath;
                    bool checkedUrl = Uri.TryCreate(item.MediaURL, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (checkedUrl)
                    {
                        localPath = uriResult.LocalPath.Remove(0, 1);
                    }
                    else
                    {
                        localPath = item.MediaURL;
                    }
                    //item.LocalPath = localPath;
                    listMedia[i].LocalPath = localPath;
                    #endregion
                    i++;
                }
            }
            //newQuestion.Media = JArray.FromObject(listMedia);
            newQuestion.Media = listMedia;
            #endregion

            #region Answers
            if (newQuestion.Answers != null && newQuestion.Answers.Count > 0)
            {
                int i = 0;
                foreach (var item in newQuestion.Answers)
                {
                    int i2 = 0;
                    foreach (var file in item.File)
                    {
                        #region Check URL
                        string localPath;
                        bool checkedUrl = Uri.TryCreate(file.MediaURL, UriKind.Absolute, out Uri uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                        if (checkedUrl)
                        {
                            localPath = uriResult.LocalPath.Remove(0, 1);
                        }
                        else
                        {
                            localPath = file.MediaURL;
                        }
                        item.File[i2].LocalPath = localPath;
                        #endregion
                        i2++;
                    }
                    newQuestion.Answers[i].File = item.File;
                    i++;
                }
            }
            #endregion

            #region Feedback
            if (newQuestion.Feedback != null && newQuestion.Feedback.File != null)
            {
                int i = 0;
                foreach (var item in newQuestion.Feedback.File)
                {
                    #region Check URL
                    string localPath;
                    bool checkedUrl = Uri.TryCreate(item.MediaURL, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (checkedUrl)
                    {
                        localPath = uriResult.LocalPath.Remove(0, 1);
                    }
                    else
                    {
                        localPath = item.MediaURL;
                    }
                    newQuestion.Feedback.File[i].LocalPath = localPath;
                    #endregion
                    i++;
                }
            }
            #endregion

            #endregion

            #region Tạo câu hỏi bên survey mongo
            newQuestion.AppId = _config["SurveyQuestion:AppId"];
            (string data, int? statusCode) resultShowAll = HttpMethod.Post.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"], Newtonsoft.Json.JsonConvert.SerializeObject(newQuestion), null).Result;

            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultShowAll.data);
            }
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);
            #endregion

            #region Add Question MariaDb
            #region Check ID Session
            string GenerateID = GeneralHelper.generateID();
            var checkIDCus = _dbContext.MiddleQuestions.Where(x => x.QuestionID == GenerateID && x.SessionID == newQuestion.SessionID).FirstOrDefault();
            if (checkIDCus != null)
            {
                while (true)
                {
                    GenerateID = GeneralHelper.generateID();
                    checkIDCus = _dbContext.MiddleQuestions.Where(x => x.QuestionID == GenerateID && x.SessionID == newQuestion.SessionID).FirstOrDefault();
                    if (checkIDCus is null)
                    {
                        break;
                    }
                }
            }
            #endregion
            var Question = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseQuestion>(result.Data.ToString());
            MiddleQuestion middleQuestion = new MiddleQuestion
            {
                MiddleID = GenerateID,
                QuestionID = Question.QuestionID,
                SessionID = newQuestion.SessionID
            };
            _dbContext.MiddleQuestions.Add(middleQuestion);
            _dbContext.SaveChanges();
            #endregion

            return (Question, "CreateQuestionSuccess");
        }
        public (object result, string message) UpdateQuestion(UpdateQuestion newQuestion, string QuestionID)
        {
            #region Check Có Submit Không Cho Update
            GetResultSubmit resultSubmit = new GetResultSubmit()
            {
                AppId = _config["SurveyQuestion:AppId"],
                QuestionID = QuestionID
            };

            //(object responseSubmit, string messageSubmit) = GetResultSubmitVoid(resultSubmit);
            //if (responseSubmit is null)
            //{
            //    return (null, messageSubmit);
            //}
            //var listResult = (List<ResponseResultSubmit>)responseSubmit;
            //if (listResult.Count > 0)
            //{
            //    return (null, "NotEditWhenAnswer");
            //}

            (object responseSubmit, string messageSubmit) = GetResultSubmitVoidByQuestion(resultSubmit);
            if (responseSubmit is null)
            {
                return (null, messageSubmit);
            }

            #endregion

            #region Check Count Answers
            if (newQuestion.CountAnswers != null && newQuestion.Type == TypeQuestion.MULTICHOOSE && newQuestion.Answers != null)
            {
                int feedback = 0;
                if (newQuestion.Feedback != null)
                {
                    feedback++;
                }
                if (newQuestion.CountAnswers > (newQuestion.Answers.Count + feedback))
                {
                    return (null, "SelectedError");
                }
                int bingo = 0;
                foreach (var item in newQuestion.Answers)
                {
                    if (item.Bingo == true)
                    {
                        bingo++;
                    }
                }
                if (newQuestion.Feedback != null && newQuestion.Feedback.Bingo == true)
                {
                    bingo++;
                }
                if (newQuestion.CountAnswers < bingo)
                {
                    return (null, "SelectedDefaultError");
                }
            }
            #endregion

            #region Custome URI

            #region MEDIA
            var listMedia = newQuestion.Media;
            if (listMedia.Count > 0)
            {
                int i = 0;
                foreach (var item in listMedia)
                {
                    #region Check URL
                    string localPath;
                    bool checkedUrl = Uri.TryCreate(item.MediaURL, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (checkedUrl)
                    {
                        localPath = uriResult.LocalPath.Remove(0, 1);
                    }
                    else
                    {
                        localPath = item.MediaURL;
                    }
                    //item.LocalPath = localPath;
                    listMedia[i].LocalPath = localPath;
                    #endregion
                    i++;
                }
            }
            //newQuestion.Media = JArray.FromObject(listMedia);
            newQuestion.Media = listMedia;
            #endregion

            #region Answers
            if (newQuestion.Answers != null && newQuestion.Answers.Count > 0)
            {
                int i = 0;
                foreach (var item in newQuestion.Answers)
                {
                    int i2 = 0;
                    foreach (var file in item.File)
                    {
                        #region Check URL
                        string localPath;
                        bool checkedUrl = Uri.TryCreate(file.MediaURL, UriKind.Absolute, out Uri uriResult)
                            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                        if (checkedUrl)
                        {
                            localPath = uriResult.LocalPath.Remove(0, 1);
                        }
                        else
                        {
                            localPath = file.MediaURL;
                        }
                        item.File[i2].LocalPath = localPath;
                        #endregion
                        i2++;
                    }
                    newQuestion.Answers[i].File = item.File;
                    i++;
                }
            }
            #endregion

            #region Feedback
            if (newQuestion.Feedback != null && newQuestion.Feedback.File != null)
            {
                int i = 0;
                foreach (var item in newQuestion.Feedback.File)
                {
                    #region Check URL
                    string localPath;
                    bool checkedUrl = Uri.TryCreate(item.MediaURL, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                    if (checkedUrl)
                    {
                        localPath = uriResult.LocalPath.Remove(0, 1);
                    }
                    else
                    {
                        localPath = item.MediaURL;
                    }
                    newQuestion.Feedback.File[i].LocalPath = localPath;
                    #endregion
                    i++;
                }
            }
            #endregion

            #endregion

            #region Send request update question mongo
            newQuestion.AppId = _config["SurveyQuestion:AppId"];
            (string data, int? statusCode) resultShowAll = HttpMethod.Put.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/{QuestionID}", Newtonsoft.Json.JsonConvert.SerializeObject(newQuestion), null).Result;
            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultShowAll.data);
            }
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);
            #endregion

            return (result.Data, "UpdateQuestionSuccess");
        }
        public (object result, string message) FlagQuestion(UpdateBeforeEvent updateBeforeEvent)
        {
            var Event = _dbContext.Events.FirstOrDefault(x => x.EventId == updateBeforeEvent.EventId);
            if (Event is null)
            {
                return (null, "EventNotExists");
            }
            var middleQuesion = _dbContext.MiddleQuestions.FirstOrDefault(x => x.QuestionID == updateBeforeEvent.QuestionId);
            GetResultSubmit getResultSubmit = new GetResultSubmit()
            {
                QuestionID = updateBeforeEvent.QuestionId
            };
            (object responseSubmit, string messageSubmit) = GetResultSubmitVoid(getResultSubmit);
            //if (responseSubmit is null)
            //{
            //    return (null, messageSubmit);
            //}
            var listResult = (List<ResponseResultSubmit>)responseSubmit;

            //get All Result by questionId
            if ((int)Event.EventFlag != (int)middleQuesion.BeforeEvent && listResult != null && listResult.Count > 0)
            {
                return (null, "PleaseEmptyResultBeforeVote");
            }
            else
            {

                if (Event.EventFlag == Events.Entities.EVENT_FLAG.BEGIN)
                {
                    if (middleQuesion != null)
                    {
                        middleQuesion.BeforeEvent = BeforeEvent.Begin;
                        _dbContext.SaveChanges();
                    }
                }

            }

            return ("UpdateQuestionSuccess", "UpdateQuestionSuccess");
        }
        public (object result, string message) UpdatePositon(UpdatePosition updatePosition)
        {
            updatePosition.AppId = _config["SurveyQuestion:AppId"];
            (string data, int? statusCode) resultShowAll = HttpMethod.Put.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/position", Newtonsoft.Json.JsonConvert.SerializeObject(updatePosition), null).Result;
            //(string data, int? statusCode) resultShowAll = HttpMethod.Put.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/ChangePosition", Newtonsoft.Json.JsonConvert.SerializeObject(updatePosition), null).Result;
            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultShowAll.data);
            }
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);
            return (result.Data, "UpdateQuestionSuccess");
        }
        public (object result, string message) DeleteQuestion(DeleteRequest delete)
        {
            #region Check Có Submit Không Cho Update
            GetResultSubmit resultSubmit = new GetResultSubmit()
            {
                AppId = _config["SurveyQuestion:AppId"],
                QuestionID = delete.QuestionID
            };

            (object responseSubmit, string messageSubmit) = GetResultSubmitVoid(resultSubmit);
            if (responseSubmit is null)
            {
                return (null, messageSubmit);
            }
            var listResult = (List<ResponseResultSubmit>)responseSubmit;
            if (listResult.Count > 0)
            {
                return (null, "NotDeleteQuestionWhenSubmit");
            }
            #endregion

            #region Check Chương trình
            var session = _dbContext.Sessions.FirstOrDefault(x => x.SessionId == delete.SessionID);
            if (session is null)
            {
                return (null, "SessionNotExists");
            }
            var events = _dbContext.Events.FirstOrDefault(x => x.EventId == session.EventId);
            if (events is null)
            {
                return (null, "EventNotExists");
            }
            //if (events.EventFlag != Events.Entities.EVENT_FLAG.CREATED)
            //{
            //    return (null, "Events ongoing or end !!");
            //}
            #endregion

            delete.AppId = _config["SurveyQuestion:AppId"];
            (string data, int? statusCode) resultShowAll = HttpMethod.Delete.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"], Newtonsoft.Json.JsonConvert.SerializeObject(delete), null).Result;
            //(string data, int? statusCode) resultShowAll = HttpMethod.Post.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + "/Delete", Newtonsoft.Json.JsonConvert.SerializeObject(delete), null).Result;
            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultShowAll.data);
            }
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);
            MiddleQuestion questions = _dbContext.MiddleQuestions.FirstOrDefault(x => x.QuestionID == delete.QuestionID && x.SessionID == delete.SessionID);
            if (questions is null)
            {
                return (null, "QuestionNotFound");
            }
            questions.Type = TypeMiddle.DELETE;
            //_dbContext.MiddleQuestions.Remove(questions);
            _dbContext.SaveChanges();
            return ("DeleteQuestionSuccess", "DeleteQuestionSuccess");
        }
        public (object result, string message) ShowAll()
        {
            (string data, int? statusCode) resultShowAll = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + "/show-all", null, null).Result;
            //(string data, int? statusCode) resultShowAll = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"],null, null).Result;

            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultShowAll.data);
            }
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);
            return (result.Data, "ShowQuestionSuccess");
        }
        public (List<ResponseQuestion> result, string message) ShowAll(string SessionID)
        {
            #region Get All Question
            var checkSession = _dbContext.Sessions.FirstOrDefault(x => x.SessionId == SessionID);
            if (checkSession is null)
            {
                return (null, "SessionNotExists");
            }

            (object res, string message) = ShowAll();
            if (res is null)
            {
                return (null, message);
            }
            var listQuestionMongo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseQuestion>>(res.ToString());
            #endregion

            var listQuestionSession = _dbContext.MiddleQuestions.Where(x => x.SessionID == SessionID && x.Type == TypeMiddle.ACTIVE).ToList();

            List<ResponseQuestion> newListResponse = new List<ResponseQuestion>();
            foreach (var item in listQuestionSession)
            {
                var getQuestion = listQuestionMongo.Where(x => x.QuestionID == item.QuestionID).FirstOrDefault();
                if (getQuestion != null)
                {
                    newListResponse.Add(getQuestion);
                }
            }
            return (newListResponse, "ShowAllQuestionSuccess");
        }
        public (List<ResponseQuestion> result, string message) ShowAllQuestionWithEvent(string EventId)
        {
            #region Get All Question
            List<ResponseQuestion> listResponseQuestion = new List<ResponseQuestion>();
            var ListSession = _dbContext.Sessions.Where(x => x.EventId == EventId).ToList();
            var listQuestionSession = new List<MiddleQuestion>();
            foreach (var item in ListSession)
            {
                var middleQuestion = _dbContext.MiddleQuestions.Where(x => x.SessionID == item.SessionId && x.Type == TypeMiddle.ACTIVE).ToList();
                listQuestionSession.AddRange(middleQuestion);
            }

            (object res, string message) = ShowAll();
            if (res is null)
            {
                return (null, message);
            }
            var listQuestionMongo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseQuestion>>(res.ToString());
            #endregion

            List<ResponseQuestion> newListResponse = new List<ResponseQuestion>();
            foreach (var item in listQuestionSession)
            {
                var getQuestion = listQuestionMongo.Where(x => x.QuestionID == item.QuestionID && x.OptionJson.Any(x => (OptionQuestion)Enum.Parse(typeof(OptionQuestion), x.ToString(), true) == OptionQuestion.SEE_THE_RESULTS_AFTER_COMPLETING_THE_EVENT)).FirstOrDefault();
                if (getQuestion != null)
                {
                    newListResponse.Add(getQuestion);
                }
            }

            return (newListResponse, "ShowAllQuestionSuccess");
        }
        public (object result, string message) GetResultSubmitVoid(GetResultSubmit getResultSubmit)
        {
            getResultSubmit.AppId = _config["SurveyQuestion:AppId"];
            (string data, int? statusCode) resultSubmit = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/submit/get-all/{getResultSubmit.AppId}", null, null).Result;
            //(string data, int? statusCode) resultSubmit = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:UrlResult"] + $"{getResultSubmit.AppId}", null, null).Result;

            if (resultSubmit.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultSubmit.data);
            }

            var responseSubmit = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultSubmit.data);

            var listResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseResultSubmit>>(responseSubmit.Data.ToString());
            if (listResult.Count == 0)
            {
                return (null, "QuestionNotSubmit");
            }
            var response = listResult.Where(x => x.QuestionID == getResultSubmit.QuestionID).ToList();
            return (response, "GetResultSubmitSuccess");
        }


        public (object result, string message) GetResultSubmitVoidByQuestion(GetResultSubmit getResultSubmit)
        {
            getResultSubmit.AppId = _config["SurveyQuestion:AppId"];
            (string data, int? statusCode) resultSubmit = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/submit/get-all/{getResultSubmit.AppId}/{getResultSubmit.QuestionID}", null, null).Result;
            //(string data, int? statusCode) resultSubmit = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:UrlResult"] + $"{getResultSubmit.AppId}", null, null).Result;

            if (resultSubmit.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultSubmit.data);
            }

            var responseSubmit = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultSubmit.data);

            var listResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseResultSubmit>>(responseSubmit.Data.ToString());
            if (listResult.Count > 0)
            {
                return (null, "NotEditWhenAnswer");
            }
            return ("Success", "GetResultSubmitSuccess");
        }

        public (List<ResponseResultSubmitQuestion> result, string message) GetResultSubmitQuestionId(GetResultSubmit getResultSubmit)
        {
            getResultSubmit.AppId = _config["SurveyQuestion:AppId"];
            (object resultSubmit, string messageSubmit) = GetResultSubmitVoid(getResultSubmit);
            if (resultSubmit is null)
            {
                return (null, messageSubmit);
            }
            var listResult = (List<ResponseResultSubmit>)resultSubmit;
            if (listResult.Count == 0)
            {
                return (new List<ResponseResultSubmitQuestion>(), "QuestionNotSubmit");
            }
            List<ResponseResultSubmitQuestion> ListResponseSubmit = new List<ResponseResultSubmitQuestion>();
            foreach (var item in listResult)
            {
                if (item?.ContentJson?.Feedback != null && item?.ContentJson?.Feedback.Selected == true)
                {
                    ListResponseSubmit.Add(new ResponseResultSubmitQuestion
                    {
                        QuestionID = item.QuestionID,
                        Feedback = item.ContentJson.Feedback,
                        User = item.ContentJson.UserID != null ? _dbContext.Users.FirstOrDefault(x => x.UserId == item.ContentJson.UserID) : null,
                        createdAt = item.createdAt,
                        updatedAt = item.updatedAt
                    });
                }
            }

            return (ListResponseSubmit, "GetResultSubmitSuccess");
        }
        public (List<ResponseResultSubmit> result, string message) GetResultSubmitDuplicate(GetResultSubmitDuplicate getResultSubmitDuplicate)
        {
            getResultSubmitDuplicate.AppId = _config["SurveyQuestion:AppId"];
            (string data, int? statusCode) resultSubmit = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/submit/get-all/{getResultSubmitDuplicate.AppId}", null, null).Result;
            //(string data, int? statusCode) resultSubmit = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:UrlResult"] + $"{getResultSubmitDuplicate.AppId}", null, null).Result;

            if (resultSubmit.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultSubmit.data);
            }
            var responseSubmit = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultSubmit.data);

            var listResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseResultSubmit>>(responseSubmit.Data.ToString());
            if (listResult.Count == 0)
            {
                return (null, "QuestionNotSubmit");
            }
            var response = listResult
                .Where(x => listResult.Any(y =>
                y.ContentJson.UserID == x.ContentJson.UserID &&
                x.ResultId != y.ResultId &&
                x.QuestionID.Equals(y.QuestionID)))
                .ToList();
            if (getResultSubmitDuplicate.QuestionId != null)
            {
                response = response.Where(x => x.QuestionID == getResultSubmitDuplicate.QuestionId).ToList();
            }

            return (response, "Success !!");
        }
        public (object result, string message) GetQuestionByList(GetQuestionByList getQuestionByList)
        {
            (string data, int? statusCode) resultShowAll = HttpMethod.Get.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/{getQuestionByList.AppId}", null, null).Result;

            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultShowAll.data);
            }
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);
            var ListQuestion = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ResponseQuestion>>(result.Data.ToString());
            List<object> listResult = new List<object>();
            if (ListQuestion.Count > 0)
            {
                foreach (var item in getQuestionByList.ListQuestionId)
                {
                    var quest = ListQuestion.Where(x => x.QuestionID == item).FirstOrDefault();
                    if (quest != null)
                    {
                        listResult.Add(quest);
                    }
                }
            }
            return (listResult, "ShowAllQuestionSuccess");
        }
        public (object result, string message) ChangeIsSent(IsSent isSent)
        {
            isSent.AppId = _config["SurveyQuestion:AppId"];
            (string data, int? statusCode) resultShowAll = HttpMethod.Post.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/change-is-sent", Newtonsoft.Json.JsonConvert.SerializeObject(isSent), null).Result;
            //(string data, int? statusCode) resultShowAll = HttpMethod.Post.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/ChangeSent", Newtonsoft.Json.JsonConvert.SerializeObject(isSent), null).Result;

            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return (null, resultShowAll.data);
            }
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);
            return (result.Data, "ChangeIsSent");
        }
        public (object result, string message) DeleteMultiSubmitOnePerson(DeleteSubmitMulti deleteSubmitMulti)
        {
            deleteSubmitMulti.AppId = _config["SurveyQuestion:AppId"];

            GetResultSubmit getResultSubmit = new GetResultSubmit();
            getResultSubmit.AppId = _config["SurveyQuestion:AppId"];
            getResultSubmit.QuestionID = deleteSubmitMulti.QuestionId;
            (object resultSubmit, string messageSubmit) = GetResultSubmitVoid(getResultSubmit);
            DeleteMultiMongo deleteMultiMongo = new DeleteMultiMongo();
            if (resultSubmit != null)
            {
                var listResult = (List<ResponseResultSubmit>)(resultSubmit);
                if (listResult.Count > 0)
                {
                    List<string> listRequestionResult = new List<string>();
                    foreach (var item in deleteSubmitMulti.ListUserId)
                    {
                        var oneResult = listResult.Where(x => x.ContentJson.UserID == item.ToString()).FirstOrDefault();
                        if (oneResult != null)
                        {
                            listRequestionResult.Add(oneResult.ResultId);
                        }
                    }
                    if (listRequestionResult.Count > 0)
                    {
                        deleteMultiMongo.ListResult = listRequestionResult;
                        (string data, int? statusCode) resultShowAll = HttpMethod.Post.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/delete-multi", Newtonsoft.Json.JsonConvert.SerializeObject(deleteMultiMongo), null).Result;
                        //(string data, int? statusCode) resultShowAll = HttpMethod.Post.SendRequestWithStringContentAsync(_config["SurveyQuestion:UrlResult"] + $"/DeleteResult/Result", Newtonsoft.Json.JsonConvert.SerializeObject(deleteMultiMongo), null).Result;

                        if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
                        {
                            return (null, resultShowAll.data);
                        }
                    }
                }
            }
            return ("ResetSubmitSuccess", "ResetSubmitSuccess");
        }
        public (object result, string message) ResetQuestion(DeleteSubmitOnePerson resetQuestion)
        {
            var middleQuestion = _dbContext.MiddleQuestions.FirstOrDefault(x => x.QuestionID.Equals(resetQuestion.QuestionId));
            if (middleQuestion is null)
            {
                return (null, "QuestionNotFound");
            }
            var Event = _dbContext.Events.FirstOrDefault(x => x.EventId.Equals(resetQuestion.EventId));
            if (Event is null)
            {
                return (null, "EventNotExists");
            }
            middleQuestion.BeforeEvent = BeforeEvent.Created;
            _dbContext.SaveChanges();
            GetResultSubmit getQuestionByList = new GetResultSubmit()
            {
                AppId = _config["SurveyQuestion:AppId"],
                QuestionID = resetQuestion.QuestionId
            };
            IsSent isSent = new IsSent()
            {
                AppId = _config["SurveyQuestion:AppId"],
                QuestionId = resetQuestion.QuestionId,
                Sent = false
            };
            ChangeIsSent(isSent);
            _author.ResetVote(resetQuestion.EventId, resetQuestion.QuestionId);
            (string data, int? statusCode) resultShowAll = HttpMethod.Delete.SendRequestWithStringContentAsync(_config["SurveyQuestion:Url"] + $"/delete-result", Newtonsoft.Json.JsonConvert.SerializeObject(getQuestionByList), null).Result;
            //(string data, int? statusCode) resultShowAll = HttpMethod.Delete.SendRequestWithStringContentAsync(_config["SurveyQuestion:UrlResult"] + $"/DeleteResult/Question", Newtonsoft.Json.JsonConvert.SerializeObject(getQuestionByList), null).Result;
            if (resultShowAll.statusCode != (int)HttpStatusCode.OK)
            {
                return ("ResetSubmitSuccess", "ResetSubmitSuccess");
            }
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseSurveyQuestion>(resultShowAll.data);
            return (result.Data, "ResetSubmitSuccess");
        }
    }
}
