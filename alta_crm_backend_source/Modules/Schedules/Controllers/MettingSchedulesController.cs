using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Schedules.Requests;
using Project.Modules.Schedules.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MettingSchedulesController : BaseController
    {
        private readonly IMeetingScheduleService MeetingScheduleService;

        public MettingSchedulesController(IMeetingScheduleService meetingScheduleService)
        {
            MeetingScheduleService = meetingScheduleService;
        }

        [HttpPost]
        public IActionResult Store([FromBody] StoreScheduleCustomerRequest request)
        {
            (object data, string message) = MeetingScheduleService.StoreMettingSchedule(request);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }
        [HttpPost("ImportCustomer")]
        public IActionResult ImportCustomer([FromForm] ImportScheduleCustomerRequest request)
        {
            (object data, string message) = MeetingScheduleService.ImportScheduleCustomer(request);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

        [HttpGet("{userId}")]
        public IActionResult GetSchedule([FromQuery] GetScheduleRequest request, string userId)
        {
            (object data, string message) = MeetingScheduleService.GetSchedule(userId, request);

            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

        [HttpGet("getMeetingSchedule")]
        public IActionResult GetScheduleByToken([FromQuery] GetScheduleRequest request)
        {
            string userId = User.FindFirst("UserId")?.Value;
            (object data, string message) = MeetingScheduleService.GetSchedule(userId, request);

            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

        [HttpGet]
        public IActionResult GetSchedules([FromQuery] GetScheduleRequest request)
        {
            var rs = MeetingScheduleService.GetAllSchedule(request);

            return ResponseOk(rs, "Success");
        }

        [HttpDelete("{scheduleId}")]
        public IActionResult Delete(string scheduleId)
        {
            (object data, string message) = MeetingScheduleService.Delete(scheduleId);

            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }
    }
}
