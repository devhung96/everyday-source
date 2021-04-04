using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.SchoolYears.Entities;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.SchoolYears.Requests;
using Project.Modules.SchoolYears.Services;
using System.Linq.Dynamic.Core;

namespace Project.Modules.SchoolYears.Controller
{
    [Route("api/school-year")]
    [ApiController]
    public class SchoolYearController : BaseController
    {
        public readonly ISchoolYearService SchoolYearService;
        public readonly IConfiguration Configuration;
        public SchoolYearController(ISchoolYearService schoolYearService, IConfiguration configuration)
        {
            SchoolYearService = schoolYearService;
            Configuration = configuration;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Entities.SchoolYear> schoolYears = SchoolYearService.GetAll();
            return ResponseOk(schoolYears);
        }
        [HttpPost("get")]
        public IActionResult GetAll([FromBody] RequestTable requestTable)
        {
            List<Entities.SchoolYear> schoolYears = SchoolYearService.GetAll();
            #region Request Table
            schoolYears = schoolYears.Where(m => String.IsNullOrEmpty(requestTable.Search) ||
                                                    (m.SchoolYearName != null && m.SchoolYearName.ToLower().RemoveUnicode().Contains(requestTable.Search.ToLower().RemoveUnicode())))
                                           .ToList();
            if (requestTable.Page < 1)
            {
                return ResponseOk(schoolYears);
            }
            if (!String.IsNullOrEmpty(requestTable.SortField) || !String.IsNullOrEmpty(requestTable.SortField))
            {
                var query = requestTable.SortField + " " + requestTable.SortOrder;
                if (!schoolYears.FieldExists<SchoolYear>(requestTable.SortField))
                {
                    return ResponseBadRequest("FieldNameError");
                }
                schoolYears = schoolYears.AsQueryable().OrderBy(query).ToList();
            }
            //else
            //{
            //    var query = "CreatedAt desc";
            //    semesters = semesters.AsQueryable().OrderBy(query).ToList();
            //}
            if (requestTable.Search == "Default" && schoolYears.Count == 0)
            {
                SchoolYear schoolYear = new SchoolYear
                {
                    SchoolYearId = "1",
                    SchoolYearName = "Default",
                    TimeStart = 2020,
                    TimeEnd = 2050
                };
                schoolYear = SchoolYearService.Insert(schoolYear);
                schoolYears.Add(schoolYear);
            }

            ResponseTable response = new ResponseTable
            {
                Data = schoolYears,
                Info = new Info
                {
                    Page = requestTable.Page,
                    Limit = requestTable.Page == -1 ? schoolYears.Count() : requestTable.Limit,
                    TotalRecord = schoolYears.Count(),
                }
            };
            #endregion
            return Ok(response); 
        }

        [HttpGet("{schoolyearId}")]
        public IActionResult GetById(string schoolyearId)
        {
            Entities.SchoolYear schoolYears = SchoolYearService.GetById(schoolyearId);
            if (schoolYears is null)
            {
                return ResponseBadRequest("SchoolYearNotFound");
            }
            return ResponseOk(schoolYears);
        }

        [HttpPost]
        public IActionResult Insert([FromBody] InsertSchoolYearRequest insertSchoolYearRequest)
        {
            Entities.SchoolYear schoolYears = new Entities.SchoolYear
            {
                SchoolYearName = insertSchoolYearRequest.SchoolYearName,
                TimeStart = insertSchoolYearRequest.TimeStart.GetValueOrDefault(),
                TimeEnd = insertSchoolYearRequest.TimeEnd.GetValueOrDefault()
            };
            SchoolYearService.Insert(schoolYears);
            return ResponseOk(schoolYears, "InsertSemesterSuccess");
        }
        [HttpDelete("{schoolYearId}")]
        public IActionResult Delete(string schoolYearId)
        {
            if (schoolYearId.Equals("75032620-cfda-4cac-81b7-ea54b2d5f7b8") || schoolYearId.Equals("1"))
            {
                return ResponseBadRequest("NotDelete");
            }
            Entities.SchoolYear schoolYears = SchoolYearService.GetById(schoolYearId);
            if (schoolYears is null)
            {
                return ResponseBadRequest("SchoolYearNotFound");
            }
            if (schoolYears.Semesters.Count > 0)
            {
                return ResponseBadRequest("ExistsData.NotDelete");
            }

            SchoolYearService.Delete(schoolYears);
            return ResponseOk("DeleteDone");
        }
        [HttpPut("{schoolYearId}")]
        public IActionResult Update([FromBody] UpdateSchoolYearRequest updateSchoolYearRequest, string schoolYearId)
        {
            Entities.SchoolYear schoolYears = SchoolYearService.GetById(schoolYearId);
            if (schoolYears is null)
            {
                return ResponseBadRequest("SchoolYearNotFound");
            }
            Entities.SchoolYear checkExists = SchoolYearService
                .GetAll()
                .FirstOrDefault(x => x.TimeEnd == updateSchoolYearRequest.TimeEnd && updateSchoolYearRequest.TimeStart == x.TimeStart);
            if (checkExists != null && checkExists.SchoolYearId != schoolYears.SchoolYearId)
            {
                return ResponseBadRequest("TimeSchoolYearIsExists");
            }
            if (schoolYears.Semesters.Count > 0)
            {
                return ResponseBadRequest("ExistsData.NotUpdate");
            }
            schoolYears = SchoolYearService.Update(schoolYears, updateSchoolYearRequest);
            if (schoolYears is null)
            {
                return ResponseBadRequest("TimeInNotValid");
            }
            return ResponseOk(schoolYears);
        }
    }
}
