using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Scores.Requests;
using Project.Modules.Scores.Services;
using Project.Modules.Subjects.Entities;

namespace Project.Modules.Scores.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportScoreController : BaseController
    {
        private readonly IReportScoreService _reportScoreService;
        private readonly ISupportScoreService supportScoreService;
        public ReportScoreController(IReportScoreService reportScoreService, ISupportScoreService supportScoreService)
        {
            _reportScoreService = reportScoreService;
            this.supportScoreService = supportScoreService;
        }


        [HttpPost("transcriptOfClassAndSubject")]
        public IActionResult TranscriptOfClassAndSubject([FromBody] TranscriptByClassRequest request)
        {
            (object result , string message) = _reportScoreService.GetTranscript(request.ClassId, request.SubjectId, request.SemestersId,request.Search);
            if (result is null) return ResponseBadRequest(message);
            return ResponseOk(result);
        }
        [HttpPost("closingPoint")]
        public IActionResult ClosingPoint([FromBody] TranscriptByClassRequest request)
        {
            (object result, string message) = _reportScoreService.ClosingPoint(request.ClassId, request.SubjectId, request.SemestersId);
            if (result is null) return ResponseBadRequest(message);
            return ResponseOk(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("DestroyClosingPoint")]
        public IActionResult DestroyClosingPoint([FromBody] TranscriptByClassRequest request)
        {
            (bool result, string message) = _reportScoreService.DestroyClosingPoint(request.ClassId, request.SubjectId, request.SemestersId);
            if (!result) return ResponseBadRequest(message);
            return ResponseOk(result);
        }


        [HttpPost("getSubjectWithClass/{classId}")]
        public IActionResult GetSubjectWithClass(string classId, [FromBody] RequestTable requestTable)
        {
            (object data, string message) = _reportScoreService.GetSubjectByClass(classId, requestTable);
            return ResponseOk(data, message);
        }



        [HttpPost("showScoreOfStudentInClass")]
        public IActionResult ShowScoreByStudentInClass([FromBody] GetScoreStudentClassSubject request)
        {
            var result = _reportScoreService.GetTranscriptByClassAndStudent(request.ClassId, request.StudentId, request.SemestersId,request.Search);
            if(result.data is null)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.data, result.message);
        }
    }
}