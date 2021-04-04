
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Requests;
using Project.Modules.Sessions.Entities;
using Project.Modules.Sessions.Requests;
using Project.Modules.Sessions.Services;
using System.Linq.Dynamic.Core;
using Project.Modules.Question.Validation;
using Project.App.Middleware;
using System;

namespace Project.Modules.Sessions.Controllers
{
    /// <summary>
    /// Còn trigger với xóa file delete
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]

    public class SessionController : BaseController
    {

        private readonly ISessionService _sessionService;

        private readonly IConfiguration _configuration;
        private readonly int isHttps = 0;

        private readonly IMapper _mapper;
        public SessionController(ISessionService sessionService, IConfiguration configuration, IMapper mapper)
        {
            _sessionService = sessionService;
            _configuration = configuration;
            isHttps = _configuration["IsHttps"].toHttps();
            _mapper = mapper;
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [DisplayNameAttribute(Modules = 6, Level = 1)]
        [HttpPost]
        public IActionResult Create([FromBody] CreateSessionRequest request)
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
            Session newSession = _mapper.Map<Session>(request);
            newSession.UserId = userId;
            newSession.EventId = idEventHeader;
            (Session data, string message, string code) = _sessionService.Create(newSession);
            if (data is null) return ResponseBadRequest(code);
            return ResponseOk(data, code);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [DisplayNameAttribute(Modules = 6, Level = 8)]
        [HttpPut("{idSession}")]
        public IActionResult Update(string idSession, [FromBody] UpdateSessionRequest request)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();
            Session newSession = _mapper.Map<Session>(request);
            newSession.EventId = idEventHeader;
            (Session data, string message, string code) = _sessionService.Update(idSession, newSession);
            if (data is null) return ResponseBadRequest(code);
            return ResponseOk(data, code);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [DisplayNameAttribute(Modules = 6, Level = 16)]
        [HttpDelete("{idSession}")]
        public IActionResult Delete(string idSession)
        {
            (bool result, string message, string code) = _sessionService.Delete(idSession);
            if (!result) return ResponseBadRequest(code);
            return ResponseOk(result, code);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [HttpGet("{idSession}")]
        public IActionResult Show(string idSession)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (Session data, string message, string code) = _sessionService.Show(idSession, urlRequest);
            if (data is null) return ResponseBadRequest(code);
            return ResponseOk(data, code);
        }

        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [DisplayNameAttribute(Modules = 6, Level = 2)]
        [HttpPost("show-all")]
        public IActionResult ShowAll([FromBody] RequestTable requestTable)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (List<Session> data, string message, string code) = _sessionService.ShowAll(urlRequest);

            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.Search) ||
                (!string.IsNullOrEmpty(requestTable.Search) &&
                    (
                        x.SessionName.ToLower().Contains(requestTable.Search.ToLower()) ||
                        x.SessionTitle.ToLower().Contains(requestTable.Search.ToLower())
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
        [DisplayNameAttribute(Modules = 6, Level = 2)]
        [HttpPost("show-all-by-event")]
        public IActionResult ShowAllByEvent([FromBody] RequestTable requestTable)
        {
            if (String.IsNullOrEmpty(Request.Headers["Event-Id"].ToString()))
            {
                return ResponseForbidden("AccountDoesNotHaveAccess(EventId)");
            }
            string idEventHeader = Request.Headers["Event-Id"].ToString();

            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (List<Session> data, string message, string code) = _sessionService.ShowByEvent(idEventHeader, urlRequest);
            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.Search) ||
                (!string.IsNullOrEmpty(requestTable.Search) &&
                    (
                        x.SessionName.ToLower().Contains(requestTable.Search.ToLower()) ||
                        x.SessionTitle.ToLower().Contains(requestTable.Search.ToLower())
                    )
                )).AsQueryable().OrderBy(OrderValue(requestTable.SortField, requestTable.SortOrder)).ToList();

            if (requestTable.Page == -1) // lay all
                requestTable.Results = data.Count();

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


        public string OrderValue(string sortColumn, string SortDir)
        {
            return sortColumn + " " + SortDir;
        }


        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [DisplayNameAttribute(Modules = 6, Level = 8)]
        [HttpGet("update-sort/{idSessionFirst}/{idSessionSecond}")]
        public IActionResult UpdateSessionSort(string idSessionFirst, string idSessionSecond)
        {
            //string idEventHeader = Request.Headers.FirstOrDefault(x => x.Key == "Event-Id").Value.FirstOrDefault();
            //if (String.IsNullOrEmpty(idEventHeader)) return ResponseForbidden("Tài khoản không có quyền truy cập");
            (bool result, string message, string code) = _sessionService.SessionSort(idSessionFirst, idSessionSecond);
            if (!result) return ResponseBadRequest(code);
            return ResponseOk(result, code);
        }



        [MiddlewareFilter(typeof(CheckLoginClient))]
        [Authorize(Roles = "ADMIN,CLIENT")]
        [DisplayNameAttribute(Modules = 6, Level = 8)]
        [HttpPost("mutiple-update-sort")]
        public IActionResult UpdateMutipleSessionSort(UpdateMutipleSessionSortRequest request)
        {
            //string idEventHeader = Request.Headers.FirstOrDefault(x => x.Key == "Event-Id").Value.FirstOrDefault();
            //if (String.IsNullOrEmpty(idEventHeader)) return ResponseForbidden("Tài khoản không có quyền truy cập");
            (bool result, string message, string code) = _sessionService.MutipleSessionSort(request);
            if (!result) return ResponseBadRequest(code);
            return ResponseOk(result, code);
        }
    }
}