using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.CourseSubjects.Entities;
using Project.Modules.CourseSubjects.Requests;
using Project.Modules.CourseSubjects.Services;

namespace Project.Modules.CourseSubjects.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseSubjectController : BaseController
    {
        private readonly ICourseSubjectService IcourseSubjectService;
        public CourseSubjectController(ICourseSubjectService _IcourseSubjectService)
        {
            IcourseSubjectService = _IcourseSubjectService;
        }
        [HttpPost("showAllCourseSubject")]
        public IActionResult ShowAllCourseSubject([FromBody] RequestTable requestTable)
        {
            (object courseSubjects, string message) = IcourseSubjectService.ShowAllCourseSubject(requestTable);
            return ResponseOk(courseSubjects, message);
        }
        [HttpPost("createCourseSubject")]
        public IActionResult CreateCourseSubject([FromBody] List<CreateCourseSubjectRequest> valueInputs)
        {
            (List<CourseSubject> courseSubject, string message) = IcourseSubjectService.CreateCourseSubject(valueInputs);
            if(courseSubject == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(courseSubject, message);
        }
        [HttpGet("showDetailCourseSubject/{courseSubjectId}")]
        public IActionResult ShowDetailCourseSubject(string courseSubjectId)
        {
            (CourseSubject courseSubject, string message) = IcourseSubjectService.ShowDetailCourseSubject(courseSubjectId);
            if (courseSubject == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(courseSubject, message);
        }
        [HttpDelete("deleteCourseSubject/{courseSubjectId}")]
        public IActionResult DeleteCourseSubject(string courseSubjectId)
        {
            (CourseSubject courseSubject, string message) = IcourseSubjectService.DeleteCourseSubject(courseSubjectId);
            if (courseSubject == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(courseSubject, message);
        }
        [HttpPut("editCourseSubject/{courseSubjectId}")]
        public IActionResult EditCourseSubject(string courseSubjectId,[FromBody] EditCourseSubjectRequest valueInput)
        {
            (CourseSubject courseSubject,List<Score> scores, string message) = IcourseSubjectService.EditCourseSubject(courseSubjectId,valueInput);
            if (courseSubject == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(courseSubject, message);
        }
        [HttpPut("editCourseSubjectV2/{courseSubjectId}")]
        public IActionResult EditCourseSubjectV2(string courseSubjectId, [FromBody] EditCourseSubjectRequest valueInput)
        {
            (CourseSubject courseSubject, List<Score> scores, string message) = IcourseSubjectService.EditCourseSubject(courseSubjectId, valueInput);
            if (courseSubject == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(new { courseSubject = courseSubject, scores = scores }, message);
        }
        [HttpPost("filterCourseSubject")]
        public IActionResult FilterCourseSubject([FromBody] FilterCourseSubjectRequest valueInput)
        {
            (List<CourseSubjectResponse> courseSubjects, string message) = IcourseSubjectService.FilterCourseSubject(valueInput);
            return ResponseOk(courseSubjects, message);
        }
    }
}