using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.ClassSchedules.Requests;
using Project.Modules.ClassSchedules.Services;
using System;
using System.Collections.Generic;

namespace Project.Modules.ClassSchedules.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ClassScheduleController : BaseController
    {
        private readonly IClassScheduleService classScheduleService;
        public ClassScheduleController(IClassScheduleService ClassScheduleService)
        {
            classScheduleService = ClassScheduleService;
        }
        [HttpPost("show")]
        public IActionResult ShowSchedule([FromBody] ShowClassScheduleRequest request)
        {
            return ResponseOk(classScheduleService.ListSchedule(request), "ShowScheduleSuccess");
        }

        [HttpPost("calendar")]
        public IActionResult ShowScheduleCalendar([FromBody] ShowCalendarRequest request)
        {
            if(string.IsNullOrEmpty(request.LecturerId) && string.IsNullOrEmpty(request.StudentId) && string.IsNullOrEmpty(request.ClassId))
            {
                string userId = User.FindFirst("AccountId").Value;
                string type = User.FindFirst("Type").Value;
                if (type.Equals("STUDENT"))
                {
                    request.StudentId = classScheduleService.DetailStudent(userId)?.StudentId;
                }
                else
                {
                    request.LecturerId = classScheduleService.DetailLecturer(userId)?.LecturerId;
                }
            }
                
            return ResponseOk(classScheduleService.ShowCalendar(request), "ShowCalendarSuccess");
        }

        [HttpPost("add")]
        public IActionResult AddSchedule([FromBody] AddScheduleRequest request)
        {
            ClassSchedule classSchedule = new ClassSchedule
            {
                ClassId = request.ClassId,
                ClassRoom = request.ClassRoom,
                DateEnd = request.DateEnd.Value,
                DateStart = request.DateStart.Value,
                LecturerId = request.LecturerId,
                SubjectId = request.SubjectId,
                ScheduleType = request.OnlineClassRoom.Value ? SCHEDULE_TYPE.ONLINE : SCHEDULE_TYPE.OFFLINE,
                StepRepeat = request.StepRepeat,
                TimeStart = TimeSpan.Parse(request.TimeStart).Ticks,
                TimeEnd = TimeSpan.Parse(request.TimeEnd).Ticks,
                DayOfWeek = string.Join(",", request.DayOfWeek)
            };

            (bool check, ClassScheduleResponse classScheduleResponse) = classScheduleService.AddSchedule(classSchedule);
            if(!check)
            {
                return ResponseBadRequest(classScheduleResponse.Error);
            }

            return ResponseOk(classScheduleResponse, "ShowScheduleSuccess");
        }

        [HttpGet("{scheduleId}")]
        public IActionResult DetailSchedule(string scheduleId)
        {
            ClassSchedule classSchedule = classScheduleService.ShowDetailCalendar(scheduleId);
            if(classSchedule is null)
            {
                return ResponseBadRequest("ScheduleNotFound");
            }
            return ResponseOk(new ClassScheduleResponse(classSchedule), "ShowScheduleDetail");
        }

        [HttpPut("{scheduleId}")]
        public IActionResult UpdateSchedule([FromBody] EditScheduleRequest request, string scheduleId)
        {
            ClassSchedule classSchedule = classScheduleService.ShowDetailCalendar(scheduleId);
            if (classSchedule is null)
            {
                return ResponseBadRequest("ScheduleNotFound");
            }
            string message;
            (classSchedule, message) = classScheduleService.EditCalendar(classSchedule, request);
            if(classSchedule is null)
            {
                return ResponseBadRequest(message);
            }
                
            return ResponseOk(new ClassScheduleResponse(classSchedule), "UpdateSuccess");
        }

        [HttpDelete("{scheduleId}")]
        public IActionResult DeleteteSchedule(string scheduleId)
        {
            ClassSchedule classSchedule = classScheduleService.ShowDetailCalendar(scheduleId);
            if (classSchedule is null)
            {
                return ResponseBadRequest("ScheduleNotFound");
            }

            classScheduleService.DeleteCalendar(new List<ClassSchedule> { classSchedule });
            return ResponseOk(null, "DeleteSuccess");
        }

        [HttpDelete("deleteBulk")]
        public IActionResult DeleteteBulkSchedule()
        {
            string classId = Request.Query["classId"].ToString();
            string subjectId = Request.Query["subjectId"].ToString();

            List<ClassSchedule> classSchedules = classScheduleService.ListSchedule(subjectId, classId);
            if(classSchedules.Count == 0)
            {
                return ResponseBadRequest("ClassIsEmpty");
            }
                
            classScheduleService.DeleteCalendar(classSchedules);
            return ResponseOk(null, "DeleteSuccess");
        }

        [HttpPost("copyClass")]
        public IActionResult CopyClass([FromBody]CopyClassRequest request)
        {
            (List<ClassScheduleResponse> data, string message) = classScheduleService.CopyClass(request);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(new {
                error = data.Count != 0,
                dataTracking = data
            }, "Copy success");
        }
    }
}
