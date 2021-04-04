using System;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Schedules.Entities;
using Project.Modules.Schedules.Services;
using Project.Modules.Schedules.Requests;
using Microsoft.AspNetCore.Authorization;
using Project.Modules.Schedules.Models;
using System.Collections.Generic;

namespace Project.Modules.Schedules.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService ScheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            ScheduleService = scheduleService;
        }

        [HttpPost]
        public IActionResult Store([FromBody] StoreScheduleNonDeviceRequest request)
        {
            (Schedule schedule, string message) = ScheduleService.Store(request);
            if (schedule == null) return ResponseBadRequest(message);
            return ResponseOk(schedule, message);
        }

        [HttpPut]
        public IActionResult Update([FromBody] UpdateScheduleRequest request)
        {
            (Schedule data, string message) = ScheduleService.Update(request);

            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

        [HttpDelete("{idSchedule}")]
        public IActionResult DeleteSchedule(string idSchedule)
        {
            (bool result, string message) = ScheduleService.Destroy(idSchedule);
            if (!result) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }


        [HttpGet]
        public IActionResult ShowAll([FromQuery] PaginationRequest requestTable)
        {
            CheckUser checkUser = new CheckUser
            {
                GroupId = User.FindFirst("group_id")?.Value,
                Level = int.Parse(User.FindFirst("is_level")?.Value)
            };

            (PaginationResponse<Schedule> schedules, string message) = ScheduleService.ShowAll(requestTable, checkUser);
            return ResponseOk(schedules, message);
        }

        [HttpGet("getScheduleByDevice")]
        public IActionResult GetScheduleByDevice([FromQuery] GetScheduleByDevice request)
        {
            (GetScheduleByDeviceResponse schedules, string message) = ScheduleService.GetScheduleByDevice(request);
            return ResponseOk(schedules, message);
        }

        [HttpGet("calendar")]
        public IActionResult Calendar([FromQuery] CalendarRequest request)
        {
            CheckUser checkUser = new CheckUser
            {
                GroupId = User.FindFirst("group_id")?.Value,
                Level = int.Parse(User.FindFirst("is_level")?.Value)
            };

            (PaginationResponse<Calendar> calendars, string message) = ScheduleService.GetCalendarForMonth(request, checkUser);
            return ResponseOk(calendars, message);
        }

        [HttpGet("{idSchedule}")]
        public IActionResult Show(string idSchedule)
        {
            (ResponseSchedule schedule, string message) = ScheduleService.Show(idSchedule);
            if (schedule == null) return ResponseBadRequest(message);
            return ResponseOk(schedule, message);
        }

        [HttpDelete]
        public IActionResult DeleteSchedule([FromBody] DeleteScheduleRequest request)
        {
            (object data, string message) = ScheduleService.Delete(request.ScheduleIds);

            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

        [HttpGet("getDeviceBySchedule/{scheduleId}")]
        public IActionResult GetDeviceBySchedule(string scheduleId)
        {
            (object data, string message) = ScheduleService.ShowDevice(scheduleId);

            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }
    }
}