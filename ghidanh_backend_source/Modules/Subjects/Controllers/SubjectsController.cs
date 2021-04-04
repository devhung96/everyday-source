using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Courses.Services;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.SubjectGroups.Services;
using Project.Modules.Subjects.Entities;
using Project.Modules.Subjects.Requests;
using System.Linq.Dynamic.Core;
using Project.Modules.Subjects.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Subjects.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : BaseController
    {
        private readonly ISubjectService _subjectService;
        private readonly ICourseService _courseService;
        private readonly ISubjectGroupService _subjectGroupService;

        public SubjectsController(ISubjectService subjectService, ICourseService courseService, ISubjectGroupService subjectGroupService)
        {
            _subjectService = subjectService;
            _courseService = courseService;
            _subjectGroupService = subjectGroupService;
        }
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            return ResponseOk(_subjectService.GetAll(), "GetAllDataSuccess");
        }

        [HttpPost("search")]
        public IActionResult SearchSubject([FromBody] SearchSubjectRequest requestTable)
        {
            ResponseTable responseTable = _subjectService.ShowAll(requestTable);
            //foreach(Subject subject in (List<Subject>)responseTable.Data)
            //{
            //    subject.CourseName = _courseService.GetCourseName(subject.CourseId);
            //    subject.SubjectGroupName = _subjectGroupService.GetSubjectGroupName(subject.SubjectGroupId);
            //}
            return ResponseOk(responseTable, "ShowAllDataSuccess");
        }

        [HttpGet("{subjectId}")]
        public IActionResult GetSubject(string subjectId)
        {
            Subject subject = _subjectService.GetById(subjectId);
            if (subject is null)
            {
                return ResponseBadRequest("SubjectNotFound");
            }
            return ResponseOk(subject, "ShowSubjectDetailSuccess");
        }

        [HttpPost("add")]
        public IActionResult StoreSubject([FromBody] StoreSubjectRequest request)
        {
            return ResponseOk(_subjectService.Store(request), "StoreNewSubjectSuccess");
        }

        [HttpPut("{subjectId}")]
        public IActionResult UpdateSubject([FromBody] UpdateSubjectRequest request, string subjectId)
        {
            Subject subject = _subjectService.GetById(subjectId);
            if (subject is null)
            {
                return ResponseBadRequest("SubjectNotFound");
            }

            if(_subjectService.CheckSubjectCodeIsExistsExceptSubject(request, subject))
            {
                return ResponseBadRequest("SubjectCodeIsExists");
            }

            subject = _subjectService.Update(request, subject);
            return ResponseOk(subject, "UpdateSubjectSuccess");
        }
        [HttpDelete("{subjectId}")]
        public IActionResult DeleteSubject(string subjectId)
        {
            Subject subject = _subjectService.GetById(subjectId);
            if (subject is null)
            {
                return ResponseBadRequest("SubjectNotFound");
            }

            _subjectService.Delete(subject);
            return ResponseOk(null, "DeleteSubjectSuccess");
        }
    }
}
