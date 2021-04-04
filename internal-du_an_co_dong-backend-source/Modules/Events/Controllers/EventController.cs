using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.App.Requests;
using Project.Modules.Events.Entities;
using Project.Modules.Events.Requests;
using Project.Modules.Events.Services;
using Project.Modules.Organizes.Entities;
using Project.Modules.Organizes.Services;
using System.Linq.Dynamic.Core;
using Project.Modules.Question.Validation;
using Project.App.Middleware;

namespace Project.Modules.Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : BaseController
    {
        private readonly IEventService _eventService;

        private readonly IConfiguration _configuration;
        private readonly int isHttps = 0;
        private readonly string  _urlMedia = "";

        private readonly IMapper _mapper;

        private readonly IOperatorService _operatorService;

        public EventController(IEventService eventService, IConfiguration configuration, IMapper mapper, IOperatorService operatorService)
        {
            _eventService = eventService;
            _configuration = configuration;
            isHttps = _configuration["IsHttps"].toHttps();
            _urlMedia = _configuration["MediaService:MediaUrl"];
            _mapper = mapper;
            _operatorService = operatorService;
        }
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpPost]
        public IActionResult Create([FromBody] CreateEventRequest request)
        {
            var user = User.FindFirst("UserId");
            if(user is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess(UserId)");
            }
            string userId = user.Value.ToString();
            // perare url
            request.EventLogoUrl = request.EventLogoUrl.GetLocalPathUrl();
            Event newEvent = _mapper.Map<Event>(request);
            newEvent.EventId = new EventId(_configuration).Value();
            newEvent.UserId = userId;
            (Event data, string message, string code) = _eventService.Create(newEvent);
            if (data is null) return ResponseBadRequest(code);
            return ResponseOk(data, code);
        }

        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpPut]
        public IActionResult Update([FromBody] UpdateEventRequest request)
        {
            // perare header
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            request.EventLogoUrl = request.EventLogoUrl.GetLocalPathUrl();
            Event newEvent = _mapper.Map<Event>(request);
            (Event data, string message, string code) = _eventService.Update(idEventHeader, newEvent,_urlMedia);
            if (data is null) return ResponseBadRequest(code);
            return ResponseOk(data,code);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpPut("update-setting")]
        [DisplayNameAttribute(Modules = 9, Level = 8)]
        public IActionResult UpdateSetting([FromBody] UpdateSettingRequest request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            (Event data, string message, string code) = _eventService.UpdateSetting(idEventHeader, request.EventSetting);
            if (data is null) return ResponseBadRequest(code);
            return ResponseOk(data, code);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpDelete]
        public IActionResult DeleteAsync()
        {
            // perare header
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            (bool result, string message, string code) = _eventService.Delete(idEventHeader);
            if (!result) return ResponseBadRequest(code);
            return ResponseOk(result, code);
        }



        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        //[DisplayNameAttribute(Modules = 2, Level = 4)]
        [HttpGet()]
        public IActionResult Show()
        {
            //string idEventHeader = Request.Headers.FirstOrDefault(x => x.Key == "Event-Id").Value.FirstOrDefault();
            //if (String.IsNullOrEmpty(idEventHeader)) return ResponseForbidden("Tài khoản không có quyền truy cập");
            if(String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();
            (Event data, string message, string code) = _eventService.Show(idEventHeader, _urlMedia);
            if (data is null) return ResponseBadRequest(code);
            return ResponseOk(data, code);
        }



        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("operator")]
        public IActionResult ShowOperator()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            (Event data, string message, string code) = _eventService.Show(idEventHeader, _urlMedia);
            if (data is null) return ResponseBadRequest(code);
            return ResponseOk(data, code);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpPost("show-all")]
        public IActionResult ShowAll([FromBody] RequestTable requestTable)
        {
            (List<Event> data, string message, string code) = _eventService.ShowAll(_urlMedia);

            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.Search) ||
                (!string.IsNullOrEmpty(requestTable.Search) &&
                    (
                        x.EventName.ToLower().Contains(requestTable.Search.ToLower()) || 
                        x.UserId.ToLower().Contains(requestTable.Search.ToLower())
                    )
                )).AsQueryable().OrderBy(OrderValue(requestTable.SortField, requestTable.SortOrder)).ToList();

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

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpPost("show-all-by-organize/{idOrganize}")]
        public IActionResult ShowEventByOrganize(string idOrganize, [FromBody] RequestTable requestTable)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (List<Event> data, string message, string code) = _eventService.ShowEventByOrganize(idOrganize, urlRequest);

            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.Search) ||
                (!string.IsNullOrEmpty(requestTable.Search) &&
                    (
                        x.EventName.ToLower().Contains(requestTable.Search.ToLower()) ||
                        x.UserId.ToLower().Contains(requestTable.Search.ToLower())
                    )
                )).ToList();

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


        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [DisplayNameAttribute(Modules = 0, Level = 2)]
        [HttpGet("get-info")]
        public IActionResult GetInfo()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            var user = User.FindFirst("UserId");
            if (user is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess(UserId)");
            }
            string userId = user.Value.ToString();
            var infoOrganize = _operatorService.GetInfoOrganize(idEventHeader, userId);
            var sessions = _operatorService.GetSessions(idEventHeader);

            var data = new
            {
                infoOrganize = infoOrganize,
                sessions = sessions
            };
            return ResponseOk(data);
        }

        /// <summary>
        /// Màn hình vận hành 
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("chart")]
        public IActionResult GetChart()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            ChartForEvent chartForEvent = _operatorService.GetChart(idEventHeader);
            return ResponseOk(chartForEvent);
        }


        /// <summary>
        /// Màn hình client 
        /// </summary>
        /// <returns></returns>
        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("chart-total")]
        public IActionResult GetChartTotal()
        {
            var _event = User.FindFirst("EventId");
            if (_event is null)
            {
                return ResponseBadRequest("AccountDoesNotHaveAccess(EventId)");
            }
            string eventId = _event.Value.ToString();
            ChartForEvent chartForEvent = _operatorService.GetChart(eventId);
            return ResponseOk(chartForEvent);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("start-event")]
        public IActionResult StartEvent()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            var token = HttpContext.Request.Headers["Authorization"].ToString();
            (bool result, string message, string messageCode) = _operatorService.BeginEvent(idEventHeader, token);
            if (!result) return ResponseBadRequest(messageCode);
            return ResponseOk(result, messageCode);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("end-event")]
        public IActionResult StopEvent()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            (bool result, string message, string messageCode) = _operatorService.EndEvent(idEventHeader, token);
            if (!result) return ResponseBadRequest(messageCode);
            return ResponseOk(result, messageCode);
        }

        public string OrderValue(string sortColumn, string SortDir)
        {
            return sortColumn + " " + SortDir;
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "CLIENT")]
        [HttpGet("check-in")]
        public IActionResult CheckIn()
        {
            var user = User.FindFirst("UserId");
            if(user is null)
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

            (object result, string message, string messageCode) = _eventService.CheckIn(userId,eventId);
            if (result is null) return ResponseBadRequest(messageCode);
            ChartForEvent chartForEvent = _operatorService.GetChart(eventId);
            return ResponseOk(chartForEvent, messageCode);
        }


        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "CLIENT")]
        [HttpGet("check-out")]
        public IActionResult CheckOut()
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
            (object result, string message, string messageCode) = _eventService.CheckOut(userId, eventId);
            if (result is null) return ResponseBadRequest(messageCode);
            ChartForEvent chartForEvent = _operatorService.GetChart(eventId);
            return ResponseOk(chartForEvent, messageCode);
        }


        [HttpGet("check-out-all")]
        public IActionResult CheckOutAll()
        {
            (bool result, string message, string messageCode) = _eventService.CheckOutAll();
            if (!result) return ResponseBadRequest(messageCode);
            return ResponseOk(result,messageCode);
            
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("status")]
        public IActionResult GetStatusEvent()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            (Event data, string message, string code) = _eventService.Show(idEventHeader, _urlMedia);
            if (data is null) return ResponseBadRequest(code);

            var result = new
            {
                status = data.EventFlag,
                note = new
                {
                    CREATED = 0,
                    BEGIN = 1,
                    END = 2
                }
            };
            return ResponseOk(result, code);

           
        }


        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("status-lading-page/{eventId}")]
        public IActionResult GetStatusEvent(string eventId)
        {
            (Event data, string message, string code) = _eventService.Show(eventId, _urlMedia);
            if (data is null) return ResponseBadRequest(code);

            var result = new
            {
                status = data.EventFlag,
                note = new
                {
                    CREATED = 0,
                    BEGIN = 1,
                    END = 2
                }
            };
            return ResponseOk(result, code);


        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("status-count-down")]
        public IActionResult GetStatusCountDownEvent()
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            (Event data, string message, string code) = _eventService.Show(idEventHeader, _urlMedia);
            if (data is null) return ResponseBadRequest(code);

            var result = new
            {
                eventId = idEventHeader,
                countDownStatus = data.EventCountDown,
                note = new
                {
                    UNALBE = 0,
                    ALBE = 1
                }
            };

            return ResponseOk(result, code);
        }


        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("status-count-down/ladingpage/{eventId}")]
        public IActionResult GetStatusCountDownEvent(string eventId)
        {
            (Event data, string message, string code) = _eventService.Show(eventId, _urlMedia);
            if (data is null) return ResponseBadRequest(code);

            var result = new
            {
                eventId = eventId,
                countDownStatus = data.EventCountDown,
                note = new
                {
                    UNALBE = 0,
                    ALBE = 1
                }
            };

            return ResponseOk(result,code);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpPut("update-count-down")]
        public IActionResult UpdateCountDown(UpdateCountDownRequest request)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();

            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();


            (object result, string message, string messageCode) = _eventService.UpdateCountDown(idEventHeader, request.CountDownStatus, token);
            if (result is null) return ResponseBadRequest(messageCode);
            return ResponseOk(result, messageCode);
        }

        [HttpGet("check-exist-event/{eventId}")]
        public IActionResult CheckEvent(string eventId)
        {
            var (result,status, message, code) = _eventService.CheckEvent(eventId);
            if(status == false)
            {
                return ResponseBadRequest(code);
            }
            return ResponseOk(result, code);
        }

    }
}