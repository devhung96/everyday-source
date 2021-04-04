using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Project.App.Controllers;
using Project.App.Middleware;
using Project.App.Requests;
using Project.Modules.Events.Entities;
using Project.Modules.Events.Services;
using Project.Modules.Medias.Services;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class QuestionUserController : BaseController
    {
        private readonly IQuestionService questionService;
        private readonly ISoketIO soketIO;

        public QuestionUserController(IQuestionService QuestionService, ISoketIO SoketIO)
        {
            questionService = QuestionService;
            soketIO = SoketIO;
        }

        private (Event, string) CheckEvent(string eventId)
        {
            Event events = questionService.DetailEvent(eventId);
            if(events is null)
            {
                return (null, "EventNotFound");
            }
            return (events, "Check Event Ok");
        }

        private (QuestionClient QuestionClient, string message) CheckQuestion(string questionId)
        {
            QuestionClient questionClient = questionService.GetQuestion(questionId);
            if(questionClient is null)
            {
                return (null, "Question Not Found");
            }
            return (questionClient, "Show Detail Success");
        }

        [HttpPost("event/{eventId}/questionClient")]
        public IActionResult ShowByEvent([FromBody] RequestTable request)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            if (!bool.TryParse(Request.Query["showAll"].ToString(), out bool showProjector))
            {
                showProjector = false;
            }
                
            (Event events, string message) checkEvent = CheckEvent(eventId);
            if(checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }
                
            (ResponseTable response, string message, List<QuestionClientSocket> socket) = questionService.ShowQuestionOnEvent(eventId, request, showProjector);

            if (showProjector)
            {
                soketIO.ForwardAsync(eventId, socket, Request.Headers["Authorization"].ToString(), "ShowAllQuestionClient", null, "0", "0", "1");
            }

            if (response is null)
                return ResponseBadRequest(message);
            return ResponseOk(response, message);
        }

        [HttpPost("event/{eventId}/questionClient/myQuestion")]
        public IActionResult ShowByUser([FromBody] RequestTable request)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            string userId = User.FindFirstValue("UserID").ToString();
            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }
            (ResponseTable response, string message) = questionService.ShowQuestionByUser(userId, request);
            if (response is null)
                return ResponseBadRequest(message);
            return ResponseOk(response, message);
        }

        [HttpGet("event/{eventId}/questionClient/{questionId}")]
        public IActionResult QuestionDetail(string questionId) 
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }

            (QuestionClient question, string message) = CheckQuestion(questionId);
            if (question is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(question, message);

        }

        //[HttpGet("questionClient")]
        //public IActionResult ShowAll()
        //{
        //    List<QuestionClient> result = questionService.ShowAll();
        //    return ResponseOk(result);
        //}

        //  [MiddlewareFilter(typeof(CheckTokenClientMiddleware))]
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [HttpPost("questionClient/add")]
        public IActionResult AddQuestion([FromBody]AddQuestionClient request)
        {
            string userId = User.FindFirstValue("UserID")?.ToString();
            string eventId = User.FindFirstValue("EventId")?.ToString();

            if (string.IsNullOrEmpty(userId))
            {
                return ResponseUnauthorized("Người dùng không tìm thấy");
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return ResponseUnauthorized("Mã sự kiện không tìm thấy");
            }

            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }

            request.EventId = eventId;
            (QuestionClient question, string message) = questionService.StoreQuestion(request, userId);
            if (question is null)
                return ResponseBadRequest(message);
            return ResponseOk(question, message);
        }

        [HttpGet("questionClient/{questionId}/allow")]
        public IActionResult AllowQuestion(string questionId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();

            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }

            (QuestionClient question, string message) checkQuestion = CheckQuestion(questionId);
            if (checkQuestion.question is null)
            {
                return ResponseBadRequest(checkQuestion.message);
            }

            checkQuestion.question.QuestionStatus = QUESTION_STATUS.ALLOW;

            (QuestionClient question, string message) = questionService.EditQuestion(checkQuestion.question);
            if (question is null)
                return ResponseBadRequest(message);

            return ResponseOk(question, message);
        }

        [HttpGet("questionClient/{questionId}/dismiss")]
        public IActionResult DismissQuestion(string questionId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();

            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }

            (QuestionClient question, string message) checkQuestion = CheckQuestion(questionId);
            if (checkQuestion.question is null)
            {
                return ResponseBadRequest(checkQuestion.message);
            }

            checkQuestion.question.QuestionStatus = QUESTION_STATUS.DISMISS;

            (QuestionClient question, string message) = questionService.EditQuestion(checkQuestion.question);
            if (question is null)
                return ResponseBadRequest(message);

            return ResponseOk(question, message);
        }

        [HttpGet("questionClient/{questionId}/answered")]
        public IActionResult AnsweredQuestion(string questionId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();

            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }

            (QuestionClient question, string message) checkQuestion = CheckQuestion(questionId);
            if (checkQuestion.question is null)
            {
                return ResponseBadRequest(checkQuestion.message);
            }

            checkQuestion.question.QuestionStatus = QUESTION_STATUS.ANSWERED;

            (QuestionClient question, string message) = questionService.EditQuestion(checkQuestion.question);
            if (question is null)
                return ResponseBadRequest(message);
            return ResponseOk(question, message);
        }

        [HttpGet("questionClient/{questionId}/active")]
        public IActionResult ChangeShowQuestion(string questionId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();

            if (!bool.TryParse(Request.Query["active"], out bool isActive))
            {
                return ResponseBadRequest("Active sai kiểu dữ liệu");
            }
                
            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }

            (QuestionClient question, string message) checkQuestion = CheckQuestion(questionId);
            if (checkQuestion.question is null)
            {
                return ResponseBadRequest(checkQuestion.message);
            }

            (QuestionClient question, string message) = questionService.ShowOrHideQuestion(checkQuestion.question, isActive);
            if (question is null)
                return ResponseBadRequest(message);
            return ResponseOk(question, message);
        }

        [HttpPut("event/{eventId}/questionClient/{questionId}")]
        public IActionResult UpdateQuesionUser([FromBody]EditQuestionClientRequest request, string questionId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }

            (QuestionClient question, string message) checkQuestion = CheckQuestion(questionId);
            if (checkQuestion.question is null)
            {
                return ResponseBadRequest(checkQuestion.message);
            }

            checkQuestion.question.QuestionContent = request.Content;

            (QuestionClient question, string message) = questionService.EditQuestion(checkQuestion.question);
            if (question is null)
                return ResponseBadRequest(message);
            return ResponseOk(question, message);
        }

        [HttpDelete("event/{eventId}/questionClient/{questionId}")]
        public IActionResult DeleteQuesionUser(string questionId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }

            (QuestionClient question, string message) checkQuestion = CheckQuestion(questionId);
            if (checkQuestion.question is null)
            {
                return ResponseBadRequest(checkQuestion.message);
            }

            (QuestionClient question, string message) = questionService.RemoveQuestion(checkQuestion.question);
            if (question is null)
                return ResponseBadRequest(message);
            return ResponseOk(question, message);
        }

        [HttpPost("event/{eventId}/questionClient/{questionId}/comment")]
        public IActionResult AddCommentToQuestion([FromBody]AddQuestionClientComment request, string questionId)
        {
            string eventId = Request.Headers["Event-Id"].ToString();
            (Event events, string message) checkEvent = CheckEvent(eventId);
            if (checkEvent.events is null)
            {
                return ResponseBadRequest(checkEvent.message);
            }

            (QuestionClient question, string message) checkQuestion = CheckQuestion(questionId);
            if (checkQuestion.question is null)
            {
                return ResponseBadRequest(checkQuestion.message);
            }

            (QuestionCommentClient questionCommentClient, string message) = questionService.CommentQuestion(new QuestionCommentClient
            {
                QuestionContent = request.Content,
                QuestionClientId = questionId,
                UserId = request.UserId
            }, checkQuestion.question);

            return ResponseOk(questionCommentClient, message);
        }

        [HttpGet("event/{eventId}/questionClient/{questionId}/comment")]
        public IActionResult ShowCommentQuestion(string questionId)
        {
            IEnumerable<QuestionCommentClient> questionCommentClient = questionService.ShowCommentQuestion(questionId);

            return ResponseOk(questionCommentClient, "Show List Comment Question");
        }
    }
}