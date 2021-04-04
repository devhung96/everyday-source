using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.ScoreTypes.Entities;
using Project.Modules.ScoreTypes.Requests;
using Project.Modules.ScoreTypes.Services;

namespace Project.Modules.ScoreTypes.Cotrollers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocreTypeSubjectController : BaseController
    {
        private readonly ISocreTypeSubjectService IsocreTypeSubjectService;
        public SocreTypeSubjectController(ISocreTypeSubjectService _IsocreTypeSubjectService)
        {
            IsocreTypeSubjectService = _IsocreTypeSubjectService;
        }
        [HttpPost("showAllSocreTypeSubject")]
        public IActionResult ShowAllSocreTypeSubject([FromBody] RequestTable requestTable)
        {
            (object data, string message) = IsocreTypeSubjectService.ShowAllSocreTypeSubject(requestTable);
            return ResponseOk(data, message);
        }
        [HttpGet("detailSocreTypeSubject/{scoreTypeSubjectId}")]
        public IActionResult DetailSocreTypeSubject(string scoreTypeSubjectId)
        {
            (ScoreTypeSubject data, string message) = IsocreTypeSubjectService.DetailSocreTypeSubject(scoreTypeSubjectId);
            if(data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data,message);
        }
        [HttpPost("createSocreTypeSubject")]
        public IActionResult CreateSocreTypeSubject([FromBody] CreateSocreTypeSubjectRequest valueInput)
        {
            (ScoreTypeSubject data, string message) = IsocreTypeSubjectService.CreateSocreTypeSubject(valueInput);
            return ResponseOk(data,message);
        }
        [HttpDelete("deleteSocreTypeSubject/{scoreTypeSubjectId}")]
        public IActionResult DeleteSocreTypeSubject(string scoreTypeSubjectId)
        {
            (ScoreTypeSubject data, string message) = IsocreTypeSubjectService.DeleteSocreTypeSubject(scoreTypeSubjectId);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPut("editSocreTypeSubject/{scoreTypeSubjectId}")]
        public IActionResult EditSocreTypeSubject(string scoreTypeSubjectId,[FromBody] EditScoreTypeSubjectRequest valueInput)
        {
            (ScoreTypeSubject data, string message) = IsocreTypeSubjectService.EditSocreTypeSubject(valueInput, scoreTypeSubjectId);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }

    }
}