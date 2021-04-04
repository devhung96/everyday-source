using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Classes.Entities;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.SubjectGroups.Entities;
using Project.Modules.SubjectGroups.Requests;
using Project.Modules.SubjectGroups.Services;
using Project.Modules.Subjects.Services;
using System.Linq.Dynamic.Core;
using static Project.Modules.Classes.Services.ClassService;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Project.Modules.Students.Services;
using Project.Modules.Students.Entities;
using Project.Modules.ClassSchedules.Entities;

namespace Project.Modules.SubjectGroups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectGroupsController : BaseController
    {
        private readonly ISubjectGroupService SubjectGroupService;
        private readonly ISubjectService _subjectService;
        private readonly IStudentService StudentService;

        public SubjectGroupsController(ISubjectGroupService subjectGroupService, ISubjectService subjectService, IStudentService studentService)
        {
            SubjectGroupService = subjectGroupService;
            _subjectService = subjectService;
            StudentService = studentService;
        }
        
        [HttpGet]
        public IActionResult GetAllSubjectGroup()
        {
            List<ClassSchedule> classSchedules = SubjectGroupService.GetClassSchedules();
            
            List<SubjectGroup> subjectGroups = SubjectGroupService
                .GetAll()
                .FilterClass(classSchedules);
            #region Lọc subject group có lớp đang mở và có lịch dạy
            List<ResponseEnrollmentClass> ClassOpen = SubjectGroupService.GetClassAllClassOpen();
            #region Lọc mấy lớp mà user đã đăng ký => không hiện lên
            string accountId = User.FindFirstValue("AccountId");
            if (accountId != null)
            {
                List<RegistrationStudy> registrationStudys = SubjectGroupService.GetRegister(accountId);
                if (registrationStudys != null)
                {
                    ClassOpen = ClassOpen
                        .Where(
                            x => !registrationStudys.Select(z => z.ClassId).ToList().Any(z => z.Contains(x.Class.ClassId)))
                        .ToList();
                }
            }
            #endregion
            subjectGroups = subjectGroups.Where(x => ClassOpen.Any(z => z.SubjectGroupId.Equals(x.SubjectGroupId))).ToList();
            #endregion
            //List<ClassSchedule> classSchedulesTemp = SubjectGroupService.GetClassSchedules();
            return ResponseOk(subjectGroups, "ShowListSubjectGroupSuccess");
        }

        [HttpPost("Search")]
        public IActionResult SearchSubjectGroup([FromBody] SearchSubjectGroupRequest requestTable)
        {
            #region Sơn Hoàng
            return ResponseOk(SubjectGroupService.ShowAll(requestTable), "ShowListSubjectGroupSuccess");
            #endregion
        }

        [HttpGet("{subjectGroupId}")]
        public IActionResult GetSubjectGroup(string subjectGroupId)
        {
            SubjectGroup subjectGroup = SubjectGroupService.GetById(subjectGroupId);
            if (subjectGroup is null)
            {
                return ResponseBadRequest("SubjectGroupNotFound");
            }
            return ResponseOk(subjectGroup, "GetDetailSuccess");
        }

        [HttpPost("add")]
        public IActionResult StoreSubjectGroup([FromBody] StoreSubjectGroupRequest request)
        {
            SubjectGroup subjectGroup = SubjectGroupService.Store(request);
            return ResponseOk(subjectGroup, "StoreNewSubjectGroupSuccess");
        }

        //[Authorize]
        [HttpPost("getClass/{subjectGroupId}")]
        public IActionResult GetClass([FromBody] RequestTable requestTable, string subjectGroupId)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            SubjectGroup subjectGroup = SubjectGroupService.GetById(subjectGroupId);
            List<ResponseEnrollmentClass> listResponseEnroll = SubjectGroupService.GetClassWithSubjectGroupId(subjectGroup);

            #region Lọc mấy lớp mà user đã đăng ký => không hiện lên
            string accountId = User.FindFirstValue("AccountId");
            if (accountId != null)
            {
                List<RegistrationStudy> registrationStudys = SubjectGroupService.GetRegister(accountId);
                if (registrationStudys != null)
                {
                    listResponseEnroll = listResponseEnroll
                        .Where(
                            x => !registrationStudys.Select(z => z.ClassId).ToList().Any(z => z.Contains(x.Class.ClassId)))
                        .ToList();
                }
            }
            #endregion

            listResponseEnroll = listResponseEnroll
                .Select(
                x =>
                {
                    x.Class.ClassImage = !String.IsNullOrEmpty(x.Class.ClassImage) ? host + x.Class.ClassImage : x.Class.ClassImage;
                    return x;
                }
                ).ToList();
            #region Request Table
            listResponseEnroll = listResponseEnroll.Where(m => String.IsNullOrEmpty(requestTable.Search) ||
                                                    (m.Class.ClassName != null && m.Class.ClassName.Contains(requestTable.Search)))
                                           .ToList();
            if (!String.IsNullOrEmpty(requestTable.SortField) || !String.IsNullOrEmpty(requestTable.SortField))
            {
                var query = requestTable.SortField + " " + requestTable.SortOrder;
                //if (!listResponseEnroll.FieldExists<Class>(requestTable.SortField))
                //{
                //    return ResponseBadRequest("FieldNameError");
                //}
                //listResponseEnroll = listResponseEnroll.AsQueryable().OrderBy(query).ToList();
            }


            ResponseTable response = new ResponseTable
            {
                Data = listResponseEnroll,
                Info = new Info
                {
                    Page = requestTable.Page,
                    Limit = requestTable.Page == -1 ? listResponseEnroll.Count() : requestTable.Limit,
                    TotalRecord = listResponseEnroll.Count(),
                }
            };
            #endregion
            return Ok(response);
            //return ResponseOk(listClass, "GetClassSuccess");
        }

        [HttpPost("getClass/register")]
        public IActionResult GetAllClassNotRegister([FromBody] RequestTable requestTable)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            List<ResponseEnrollmentClass> listResponseEnroll = SubjectGroupService.GetClassAllClassOpen();

            #region Lọc mấy lớp mà user đã đăng ký => không hiện lên
            string accountId = User.FindFirstValue("AccountId");
            if (accountId != null)
            {
                List<RegistrationStudy> registrationStudys = SubjectGroupService.GetRegister(accountId);
                if (registrationStudys != null)
                {
                    listResponseEnroll = listResponseEnroll
                        .Where(
                            x => !registrationStudys.Select(z => z.ClassId).ToList().Any(z => z.Contains(x.Class.ClassId)))
                        .ToList();
                }
            }
            #endregion

            listResponseEnroll = listResponseEnroll
                .Select(
                x =>
                {
                    x.Class.ClassImage = !String.IsNullOrEmpty(x.Class.ClassImage) ? host + x.Class.ClassImage : x.Class.ClassImage;
                    return x;
                }
                ).ToList();
            #region Request Table
            listResponseEnroll = listResponseEnroll.Where(m => String.IsNullOrEmpty(requestTable.Search) ||
                                                    (m.Class.ClassName != null && m.Class.ClassName.Contains(requestTable.Search)))
                                           .ToList();
            if (!String.IsNullOrEmpty(requestTable.SortField) || !String.IsNullOrEmpty(requestTable.SortField))
            {
                var query = requestTable.SortField + " " + requestTable.SortOrder;
                //if (!listResponseEnroll.FieldExists<Class>(requestTable.SortField))
                //{
                //    return ResponseBadRequest("FieldNameError");
                //}
                //listResponseEnroll = listResponseEnroll.AsQueryable().OrderBy(query).ToList();
            }


            ResponseTable response = new ResponseTable
            {
                Data = listResponseEnroll,
                Info = new Info
                {
                    Page = requestTable.Page,
                    Limit = requestTable.Page == -1 ? listResponseEnroll.Count() : requestTable.Limit,
                    TotalRecord = listResponseEnroll.Count(),
                }
            };
            #endregion
            return Ok(response);
        }

        [HttpGet("getClass/{subjectGroupId}/detail/{classScheduleId}")]
        public IActionResult GetClassFirst(string classScheduleId, string subjectGroupId)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            SubjectGroup subjectGroup = SubjectGroupService.GetById(subjectGroupId);
            if (subjectGroup is null)
            {
                return ResponseBadRequest("SubjectGroupNotFound");
            }
            List<ResponseEnrollmentClass> listResponseEnrolls = SubjectGroupService.GetClassWithSubjectGroupId(subjectGroup);
            ResponseEnrollmentClass ResponseEnroll = listResponseEnrolls
                .Where(x => x.ClassSchedule != null && x.ClassSchedule.ClassScheduleId.Equals(classScheduleId))
                .Select(
                x =>
                {
                    x.Class.ClassImage = !String.IsNullOrEmpty(x.Class.ClassImage) ? host + x.Class.ClassImage : x.Class.ClassImage;
                    return x;
                }
                ).FirstOrDefault();
            if (ResponseEnroll is null)
            {
                return ResponseBadRequest("ClassNotFound");
            }
            return ResponseOk(ResponseEnroll, "GetClassSuccess");
        }

        [HttpPut("{subjectGroupId}")]
        public IActionResult UpdateSubjectGroup([FromBody] UpdateSubjectGroupRequest request, string subjectGroupId)
        {
            SubjectGroup subjectGroup = SubjectGroupService.GetById(subjectGroupId);

            if (subjectGroup is null)
            {
                return ResponseBadRequest("SubjectGroupNotFound");
            }

            subjectGroup = SubjectGroupService.Update(request, subjectGroup);
            return ResponseOk(subjectGroup, "UpdateSubjectGroup");
        }

        [HttpDelete("{subjectGroupId}")]
        public IActionResult DeleteSubjectGroup(string subjectGroupId)
        {
            SubjectGroup subjectGroup = SubjectGroupService.GetById(subjectGroupId);
            if (subjectGroup is null)
            {
                return ResponseBadRequest("SubjectGroupNotFound");
            }
            if (_subjectService.CheckSubjectIsContainedInSubjectGroup(subjectGroupId))
            {
                return ResponseBadRequest("ThisSubjectGroupContainSubject");
            }
            SubjectGroupService.Delete(subjectGroup);

            return ResponseOk(null, "DeleteSuccess");
        }
    }
}
