using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoogleTranslateFreeApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Middleware;
using Project.App.Requests;
using Project.Modules.Question.Request;
using Project.Modules.Question.Response;
using Project.Modules.Question.Services;
using Project.Modules.Question.Validation;

namespace Project.Modules.Question.Controller
{
    [Route("api/question")]
    [ApiController]
    public class QuestionShareHolderController : BaseController
    {
        public readonly IQuestionAnswersService _questAnswer;
        public readonly ISubmitQuestionService _submitQuestion;
        public readonly IConfiguration _config;
        public QuestionShareHolderController(IQuestionAnswersService questionAnswers, IConfiguration config, ISubmitQuestionService submitQuestion)
        {
            _questAnswer = questionAnswers;
            _config = config;
            _submitQuestion = submitQuestion;
           
        }
        [DisplayNameAttribute]
        [HttpPost("show-all")]
        public IActionResult ShowAll()
        {
            (object result, string message) = _questAnswer.ShowAll();
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }

        //[DisplayNameAttribute(Modules = 7,Level = 2)]
        //[DisplayNameAttribute(Modules = 0, Level = 2)]
        [HttpPost("session/{sessionID}")]
        public IActionResult ShowWithSession([FromBody]RequestTable requestTable, string sessionID)
        {
            (List<ResponseQuestion> data, string message) = _questAnswer.ShowAll(sessionID);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            if (String.IsNullOrEmpty(requestTable.SortField) || String.IsNullOrEmpty(requestTable.SortOrder))
            {
                requestTable.SortField = "CreatedAt";
                requestTable.SortOrder = "asc";
            }

            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.Search) ||
                (!string.IsNullOrEmpty(requestTable.Search) &&
                    (
                        x.Content.ToLower().Contains(requestTable.Search.ToLower())
                    )
                ))
                .OrderBy(x => x.CreatedAt)
                .ToList();

            ResponseTable responseTable = new ResponseTable()
            {
                DateResult = data.Skip((requestTable.Page - 1) * requestTable.Results).Take(requestTable.Results).ToList(),
                Info = new Info()
                {
                    Page = requestTable.Page,
                    TotalRecord = data.Count,
                    Results = requestTable.Results
                }
            };
            #endregion
            return Ok(responseTable);
        }

        [DisplayNameAttribute(Modules = 7, Level = 1)]
        [HttpPost]
        public IActionResult StoreQuestion([FromBody]NewQuestion newQuestion)
        {
            (object result, string message) = _questAnswer.StoreQuestion(newQuestion);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        [DisplayNameAttribute(Modules = 7, Level = 8)]
        [HttpPut("{questionID}")]
        public IActionResult UpdateQuestion([FromBody]UpdateQuestion updateQuestion,string questionID)
        {
            (object result, string message) = _questAnswer.UpdateQuestion(updateQuestion, questionID);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        [DisplayNameAttribute(Modules = 7, Level = 16)]
        [HttpDelete]
        [MiddlewareFilter(typeof(CheckLoginClient))]
        public IActionResult DeleteQuestion([FromQuery] DeleteRequest request)
        {
            (object result, string message) = _questAnswer.DeleteQuestion(request);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }

        //revote
        //[Authorize]
        [DisplayNameAttribute]
        [HttpPost("revote")]
        public IActionResult ListRevote([FromBody] ListRevote listRevote)
        {
            var EventId = HttpContext.Request.Headers["Event-Id"].ToString();
            if (String.IsNullOrEmpty(EventId))
            {
                return ResponseBadRequest("EventIdRequired");
            }
            listRevote.EventId = EventId;
            (object result, string message) = _submitQuestion.GetUserRevote(listRevote);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        [DisplayNameAttribute]
        [HttpPost("change-is-sent")]
        public IActionResult ChangeIsSent([FromBody] IsSent isSent)
        {
            (object result, string message) = _questAnswer.ChangeIsSent(isSent);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }

        //Submit
        [Authorize]
        [HttpPost("submit")]
        [MiddlewareFilter(typeof(CheckLoginClient))]
        public IActionResult SubmitAnswers([FromBody]SubmitQuestion submitQuestion)
        {
            string userId = User.FindFirst("UserId").Value.ToString();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            string eventId = User.FindFirst("EventId").Value.ToString();
            submitQuestion.EventID = eventId;
            //submitQuestion.Token = token;
            submitQuestion.UserID = userId;
            //Thread.Sleep(15000);
            (object result, string message) = _submitQuestion.SubmitSingleQuestion(submitQuestion,token);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        [DisplayNameAttribute(Modules = 0, Level = 2)]
        [HttpGet("submit/{organizationID}")]
        public IActionResult ResultSubmit(string organizationID)
        {
            (object result, string message) = _submitQuestion.GetAllResultOrganization(organizationID);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        [DisplayNameAttribute(Modules = 0, Level = 2)]
        [HttpPost("submit-result")]
        public IActionResult ResultSubmitSession([FromBody]GetResultSubmit getResultSubmit)
        {
            (object result, string message) = _questAnswer.GetResultSubmitVoid(getResultSubmit);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        
        [DisplayNameAttribute(Modules = 0, Level = 2)]
        [HttpPost("result-submit-question/{questionId}")]
        public IActionResult ResultSubmitQuestion([FromBody]RequestTable requestTable, string questionId)
        {
            GetResultSubmit getResultSubmit = new GetResultSubmit()
            {
                QuestionID = questionId
            };
            (List<ResponseResultSubmitQuestion> data, string message) = _questAnswer.GetResultSubmitQuestionId(getResultSubmit);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            if (String.IsNullOrEmpty(requestTable.SortField) || String.IsNullOrEmpty(requestTable.SortOrder))
            {
                requestTable.SortField = "CreatedAt";
                requestTable.SortOrder = "asc";
            }

            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.Search) ||
                (!string.IsNullOrEmpty(requestTable.Search) &&
                    (
                        x.Feedback.Content.ToLower().Contains(requestTable.Search.ToLower())
                    )
                ))
                .OrderBy(x => x.createdAt)
                .ToList();

            ResponseTable responseTable = new ResponseTable()
            {
                DateResult = data.Skip((requestTable.Page - 1) * requestTable.Results).Take(requestTable.Results).ToList(),
                Info = new Info()
                {
                    Page = requestTable.Page,
                    TotalRecord = data.Count,
                    Results = requestTable.Results
                }
            };
            #endregion
            return Ok(responseTable);
        }

        //[DisplayNameAttribute(Modules = 7, Level = 2)]
        [HttpPost("get-by-list")]
        public IActionResult GetQuestionByListString([FromBody]GetQuestionByList getQuestionByList)
        {
            (object result, string message) = _questAnswer.GetQuestionByList(getQuestionByList);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        
        [DisplayNameAttribute(Modules = 7, Level = 8)]
        [HttpPut("position")]
        public IActionResult UpdatePosition([FromBody]UpdatePosition updatePosition)
        {
            (object result, string message) = _questAnswer.UpdatePositon(updatePosition);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }



        
        //[DisplayNameAttribute(Modules = 0, Level = 2)]
        //[HttpPost("one-for-all")]
        //[MiddlewareFilter(typeof(CheckLoginClient))]
        //public IActionResult AnswerForAll([FromBody]FlagSubmitAll flagSubmitAll)
        //{
        //    var EventId = HttpContext.Request.Headers["Event-Id"].ToString();
        //    if (String.IsNullOrEmpty(EventId))
        //    {
        //        return ResponseBadRequest("EventId không được bỏ trống.");
        //    }
        //    flagSubmitAll.EventId = EventId;
        //    var token = HttpContext.Request.Headers["Authorization"].ToString();
        //    (object result, string message) = _submitQuestion.FlagSubmitAll(flagSubmitAll, token);
        //    if (result is null)
        //    {
        //        return ResponseBadRequest(message);
        //    }
        //    return ResponseOk(result, message);
        //}
        
        [DisplayNameAttribute(Modules = 0, Level = 2)]
        [HttpPost("reset-question")]
        public IActionResult ResetQuestion([FromBody] DeleteSubmitOnePerson resetQuestion)
        {
            var EventId = HttpContext.Request.Headers["Event-Id"].ToString();
            if (String.IsNullOrEmpty(EventId))
            {
                return ResponseBadRequest("EventIdRequired");
            }
            resetQuestion.EventId = EventId;
            (object result, string message) = _questAnswer.ResetQuestion(resetQuestion);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        
        [DisplayNameAttribute(Modules = 0, Level = 2)]
        [HttpPost("delete-multi-result")]
        [MiddlewareFilter(typeof(CheckLoginClient))]
        public IActionResult DeleteMultiResult([FromBody] DeleteSubmitMulti deleteSubmitMulti)
        {
            (object result, string message) = _questAnswer.DeleteMultiSubmitOnePerson(deleteSubmitMulti);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }

        //[DisplayNameAttribute(Modules = 7, Level = 2)]
        [HttpPost("question-event/{eventId}")]
        public IActionResult ShowQuestionEvent(string eventId)
        {
            (object result, string message) = _questAnswer.ShowAllQuestionWithEvent(eventId);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
        [DisplayNameAttribute]
        [HttpPost("change-flag-event")]
        public IActionResult ChangeFlagEvent([FromBody]UpdateBeforeEvent updateBeforeEvent)
        {
            var EventId = HttpContext.Request.Headers["Event-Id"].ToString();
            if (String.IsNullOrEmpty(EventId))
            {
                return ResponseBadRequest("EventIdRequired");
            }
            updateBeforeEvent.EventId = EventId;
            (object result, string message) = _questAnswer.FlagQuestion(updateBeforeEvent);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }

        [HttpPost("question-duplicate")]
        public IActionResult Duplicate([FromBody]GetResultSubmitDuplicate resultSubmitDuplicate)
        {
            (object result, string message) = _questAnswer.GetResultSubmitDuplicate(resultSubmitDuplicate);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, message);
        }
    }
}