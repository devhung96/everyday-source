using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Middleware;
using Project.App.Requests;
using Project.Modules.Events.Services;
using Project.Modules.Organizes.Entities;
using Project.Modules.Organizes.Services;
using Project.Modules.Question.Request;
using Project.Modules.Question.Response;
using Project.Modules.Question.Validation;
using Project.Modules.Reports.Entities;
using Project.Modules.Reports.Requests;
using Project.Modules.Reports.Services;

namespace Project.Modules.Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : BaseController
    {
        public readonly IReportService _reportService;
        public readonly IEndMeetingReportService endMeetingReportService;
        public readonly ISoketIO _soketIO;

        private readonly IOperatorService _operatorService;
        public ReportController(IReportService reportService, IEndMeetingReportService EndMeetingReportService, ISoketIO soketIO, IOperatorService operatorService)
        {
            _reportService = reportService;
            endMeetingReportService = EndMeetingReportService;
            _soketIO = soketIO;
            _operatorService = operatorService;
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [DisplayNameAttribute(Modules = 11, Level = 2)]
        [HttpGet("export")]
        public IActionResult ExportReport()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();
            Report data = _reportService.ExportReport(idEventHeader);
            return ResponseOk(data);
        }


        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpPost("export-datatable")]
        public IActionResult ExportReportDataTable([FromBody] RequestTable requestTable)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            object data = _reportService.ExportReportDataTable(idEventHeader, requestTable); 
            return Ok(data);
        }


        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("result-question-chart/{questionId}")]
        public IActionResult QuestionReport(string questionId)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();
            object data = _reportService.GetChartWithQuestion(questionId, idEventHeader);
            return ResponseOk(data);
        }


        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("stock-question-chart/{questionId}")]
        public IActionResult StockQuestionChart(string questionId)
        {
            object data = _reportService.GetChartStockWithQuestion(questionId);
            return ResponseOk(data);
        }





        //cms

        /// <summary>
        /// Màn hình vận hành
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("question-chart/{questionId}")]
        public IActionResult QuestionChart(string questionId)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();
            var data = _reportService.GeneratorDataChartResponse(idEventHeader, questionId);
            return ResponseOk(data);
        }


        //langding page
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("question-chart/{questionId}/{eventId}")]
        public IActionResult QuestionChart(string questionId, string eventId)
        {
            var data = _reportService.GeneratorDataChartResponse(eventId, questionId);
            return ResponseOk(data);
        }


        /// <summary>
        /// Màn hình client
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        /// 
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("question-chart-ladingpage/{questionId}")]
        public IActionResult QuestionChartClient(string questionId)
        {

            var _event = User.FindFirst("EventId");
            if (_event is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess(EventId)");
            }
            string eventId = _event.Value.ToString();

            var data =  _reportService.GeneratorDataChartResponse(eventId,questionId);
            return ResponseOk(data);
        }


        /// <summary>
        /// Show lai chart màn hình vận hành
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("show-chart/{questionId}")]
        public IActionResult ShowChart(string questionId)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            var dataSendSocket = _reportService.GeneratorDataChartResponse(idEventHeader, questionId, true, "displayShowChart(Hiện thị chart khi màn hình vận hành ấn):  true = show , false = hidden");
            _soketIO.ForwardAsync(idEventHeader, dataSendSocket, token, "realtimeChart", null, "1", "1");
            return ResponseOk(dataSendSocket);

        }





        // Báo cáo kết thúc cuộc họp
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("endMeeting")]
        public IActionResult ReportEndMeeting()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            (ReportEndMeeting data, string messageCode) = endMeetingReportService.ReportEndMeeting(idEventHeader);
            if(data is null)
            {
                return ResponseBadRequest(messageCode);
            }
            return ResponseOk(data, messageCode);
        }

        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("send-chart-close-question/{questionId}")]
        public IActionResult CloseQuestion(string questionId)
        {

            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            var token = HttpContext.Request.Headers["Authorization"].ToString();

            #region Socket dev hung

            ResponseQuestion question = _reportService.GetQuestionFromSurvey(questionId);


            if (question.OptionJson.Any(x => int.Parse(x.ToString()) == (int)OptionQuestion.SEE_RESULTS_RIGHT_AWAY))
            {
                _reportService.SubmitSocketChartAsync(idEventHeader, questionId,token, OptionQuestion.SEE_RESULTS_RIGHT_AWAY);
            }
            else
            {
                _reportService.SubmitSocketChartAsync(idEventHeader,questionId, token, null);
            }
            return ResponseOk(null,"Success");
           
            #endregion
        }
    }
}