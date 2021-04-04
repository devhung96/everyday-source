using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.Modules.Organizes.Entities;
using Project.Modules.Organizes.Services;

namespace Project.Modules.Organizes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorController : BaseController
    {
        private readonly IOperatorService _operatorService;

        public OperatorController(IOperatorService operatorService)
        {
            _operatorService = operatorService;
          
        }


        [Authorize(Roles = "CLIENT")]
        [HttpGet]
        public IActionResult GetInfo()
        {

            var user = User.FindFirst("UserId");
            if (user is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess(UserId)");
            }
            var _event = User.FindFirst("EventId");
            if (_event is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess(EventId)");
            }

            string userId = user.Value.ToString();
            string eventId = _event.Value.ToString();

            var infoShareholder = _operatorService.GetInfoShareholder(userId, eventId);
            var infoOrganize = _operatorService.GetInfoOrganize(eventId, userId);
            var documentOrganize = _operatorService.GetDocumentOrganize(eventId);
            var sessions = _operatorService.GetSessions(eventId);

            var data = new
            {
                infoShareholder = infoShareholder,
                infoOrganize = infoOrganize,
                documentOrganize = documentOrganize,
                sessions = sessions
            };
            return ResponseOk(data);
        }


        [Authorize(Roles = "CLIENT")]
        [HttpGet("chart")]
        public IActionResult GetChart()
        {
            var _event = User.FindFirst("EventId");
            if (_event is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess(EventId)");
            }
            string eventId = _event.Value.ToString();
            ChartForEvent chartForEvent =  _operatorService.GetChart(eventId);
            return ResponseOk(chartForEvent);
        }


        [Authorize(Roles = "CLIENT")]
        [HttpGet("start-event")]
        public IActionResult StartEvent()
        {
            var _event = User.FindFirst("EventId");
            if (_event is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess(EventId)");
            }
            string eventId = _event.Value.ToString();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            (bool result , string message, string messageCode) = _operatorService.BeginEvent(eventId,token);
            if (!result) return ResponseBadRequest(messageCode);
            return ResponseOk(result, messageCode);
        }

        [Authorize(Roles = "CLIENT")]
        [HttpGet("end-event")]
        public IActionResult StopEvent()
        {
            var _event = User.FindFirst("EventId");
            if (_event is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess(EventId)");
            }
            string eventId = _event.Value.ToString();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            (bool result, string message, string messageCode) = _operatorService.EndEvent(eventId,token);
            if (!result) return ResponseBadRequest(message, messageCode);
            return ResponseOk(result, messageCode);
        }


        

    }
}