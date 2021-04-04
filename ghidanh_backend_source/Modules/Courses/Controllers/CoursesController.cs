using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Project.App.Controllers;
using Project.Modules.Courses.Entities;
using Project.Modules.Courses.Requests;
using Project.Modules.Courses.Services;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.Subjects.Services;
using System.Linq.Dynamic.Core;
using Project.App.Helpers;

namespace Project.Modules.Courses.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : BaseController
    {
        private readonly ICourseService _courseService;
        private readonly ISubjectService _subjectService;

        public CoursesController(ICourseService courseService, ISubjectService subjectService)
        {
            _courseService = courseService;
            _subjectService = subjectService;
        }
        [HttpPost]
        public IActionResult StoreCourse([FromBody] StoreCourseRequest request)
        {
            return ResponseOk(_courseService.Store(request), "StoreNewCubjectSuccess");
        }
        [HttpPost("Search")]
        public IActionResult SearchCourse([FromBody] SearchCourseRequest requestTable)
        {
            List<Course> courses = _courseService.GetAll();
            #region Request Table
            courses = courses.Where(m => String.IsNullOrEmpty(requestTable.Search) ||
                                                    (m.CourseName != null && m.CourseName.ToLower().RemoveUnicode().Contains(requestTable.Search.ToLower().RemoveUnicode()))
                                                                                    ||
                                                    (m.CourseCode != null && m.CourseCode.ToLower().RemoveUnicode().Contains(requestTable.Search.ToLower().RemoveUnicode()))
                                                    )
                                           .ToList();
            if (requestTable.Page < 1)
            {
                return ResponseOk(courses);
            }
            if (!String.IsNullOrEmpty(requestTable.SortField) || !String.IsNullOrEmpty(requestTable.SortField))
            {
                var query = requestTable.SortField + " " + requestTable.SortOrder;
                if (!courses.FieldExists<Course>(requestTable.SortField))
                {
                    return ResponseBadRequest("FieldNameError");
                }
                courses = courses.AsQueryable().OrderBy(query).ToList();
            }
            ResponseTable response = new ResponseTable
            {
                Data = requestTable.Page != 0 ? courses.Skip((requestTable.Page - 1) * requestTable.Limit).Take(requestTable.Limit).ToList() : courses.ToList(),
                Info = new Info
                {
                    Page = requestTable.Page != 0 ? requestTable.Page : 1,
                    Limit = requestTable.Page != 0 ? requestTable.Limit : courses.Count(),
                    TotalRecord = courses.Count(),
                }
            };
            #endregion
            //return ResponseOk(_courseService.ShowAll(requestTable), "ShowListCourseSuccess");
            return ResponseOk(response, "ShowListCourseSuccess");
        }
        [HttpPut("{courseId}")]
        public IActionResult UpdateCourse([FromBody] UpdateCourseRequest request, string courseId)
        {
            Course course = _courseService.GetById(courseId);
            if (course is null)
            {
                return ResponseBadRequest("ThisCourseIsNotExist");
            }
            if (_courseService.CheckCourseCodeIsExistsExceptCourse(request, course))
            {
                return ResponseBadRequest("ThisCourseCodeIsExist");
            }
            course = _courseService.Update(request, course);
            return ResponseOk(course, "UpdateCourseSuccess");
        }

        [HttpDelete("{courseId}")]
        public IActionResult DeleteCourse(string courseId)
        {
            if (courseId.Equals("1"))
            {
                return ResponseBadRequest("CourseDefaultCanNotBeDeleted");
            }
            Course course = _courseService.GetById(courseId);
            if (course is null)
            {
                return ResponseBadRequest("CourseNotFound");
            }

            _courseService.Delete(course);

            return ResponseOk(null, "DeleteCourseSuccess");
        }

        [HttpGet]
        public IActionResult GetAllCourse()
        {
            return ResponseOk(_courseService.GetAll(), "Success");
        }
        [HttpGet("{courseId}")]
        public IActionResult GetCourse(string courseId)
        {
            Course course = _courseService.GetById(courseId);
            if (course is null)
            {
                return ResponseBadRequest("CourseNotFound");
            }
            return ResponseOk(course, "ShowCourseDetailSuccess");
        }
    }

}
