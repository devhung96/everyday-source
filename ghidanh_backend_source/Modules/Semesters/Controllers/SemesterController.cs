using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Semesters.Entities;
using Project.Modules.Semesters.Requests;
using Project.Modules.Semesters.Services;
using System.Linq.Dynamic.Core;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.Scores.Services;
using System.Diagnostics;
using System.Globalization;

namespace Project.Modules.Semesters.Controllers
{
    [Route("api/semester")]
    [ApiController]
    public class SemesterController : BaseController
    {
        public readonly ISemesterService SemesterService;
        public readonly IScoreService ScoreService;
        public readonly IConfiguration Configuration;

        public SemesterController(ISemesterService semesterService, IConfiguration configuration, IScoreService scoreService)
        {
            SemesterService = semesterService;
            Configuration = configuration;
            ScoreService = scoreService;
        }

        
        [HttpPost("get")]
        public IActionResult GetAll([FromBody] RequestTable requestTable)
        {
            List<Semester> semesters = SemesterService.GetAll();
            #region Request Table
            semesters = semesters.Where(m => String.IsNullOrEmpty(requestTable.Search) ||
                                                    (m.SemesterName != null && m.SemesterName.Contains(requestTable.Search)))
                                           .ToList();
            if (!String.IsNullOrEmpty(requestTable.SortField) || !String.IsNullOrEmpty(requestTable.SortField))
            {
                var query = requestTable.SortField + " " + requestTable.SortOrder;
                if (!semesters.FieldExists<Semester>(requestTable.SortField))
                {
                    return ResponseBadRequest("FieldNameError");
                }
                semesters = semesters.AsQueryable().OrderBy(query).ToList();
            }
            //else
            //{
            //    var query = "CreatedAt desc";
            //    semesters = semesters.AsQueryable().OrderBy(query).ToList();
            //}


            ResponseTable response = new ResponseTable
            {
                Data = semesters,
                Info = new Info
                {
                    Page = requestTable.Page,
                    Limit = requestTable.Page == -1 ? semesters.Count() : requestTable.Limit,
                    TotalRecord = semesters.Count(),
                }
            };
            #endregion
            return Ok(response);
        }

        [HttpGet("{semesterId}")]
        public IActionResult GetById(string semesterId)
        {
            Semester semester = SemesterService.GetById(semesterId);
            if (semester is null)
            {
                return ResponseBadRequest("SemesterNotFound");
            }
            return ResponseOk(semester);
        }

        [HttpPost]
        public IActionResult Insert([FromBody] InsertSemesterRequest insertSemester)
        {
            Semester semester = new Semester
            {
                SemesterName = insertSemester.SemesterName,
                //TimeEnd = DateTime.Parse(insertSemester.TimeEnd),
                TimeEnd = DateTime.ParseExact(insertSemester.TimeStart, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                TimeStart = DateTime.ParseExact(insertSemester.TimeEnd, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                SchoolYearId = insertSemester.SchoolYearId
            };
            if (!semester.CheckConditionSemester())
            {
                return ResponseBadRequest("TimeStartIsInvalid");
            }
            SemesterService.Insert(semester);
            return ResponseOk(semester);
        }

        [HttpPut("{semesterId}")]
        public IActionResult Update([FromBody]UpdateSemesterRequest updateSemester, string semesterId)
        {
            Semester semester = SemesterService.GetById(semesterId);
            if (semester is null)
            {
                return ResponseBadRequest("SemesterNotFound");
            }
            if(ScoreService.ShowAll().FirstOrDefault(x => x.SemestersId.Equals(semester.SemesterId)) != null)
            {
                return ResponseBadRequest("DataClassExists.NotUpdate");
            }

            semester = SemesterService.Update(semester, updateSemester);
            if (semester is null)
            {
                return ResponseBadRequest("TimeStartInvalid");
            }
            return ResponseOk(semester);
        }

        [HttpDelete("{semesterId}")]
        public IActionResult Delete(string semesterId)
        {
            if (semesterId.Equals("5834ecea-17d5-46ea-8de0-b12812bcfd85"))
            {
                return ResponseBadRequest("NotDelete");
            }
            Semester semester = SemesterService.GetById(semesterId);
            if (semester is null)
            {
                return ResponseBadRequest("SemesterNotFound");
            }
            if (ScoreService.ShowAll().FirstOrDefault(x => x.SemestersId.Equals(semester.SemesterId)) != null)
            {
                return ResponseBadRequest("DataClassExists.NotDelete");
            }
            SemesterService.Delete(semester);
            return ResponseOk("DeleteSemesterSuccess");
        }

    }
}
