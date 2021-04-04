using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Scores.Requests;
using Project.Modules.Scores.Services;
using System.Collections.Generic;

namespace Project.Modules.Scores.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : BaseController
    {
        private readonly IScoreService IscoreService;
        public ScoreController(IScoreService _IscoreService)
        {
            IscoreService = _IscoreService;
        }
        [HttpPost("createScore")]
        public IActionResult CreateScore([FromBody] List<CreateScoreRequest> valueInputs)
        {
            (object data, string message) = IscoreService.CreateScore(valueInputs);
            if(data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPost("showAllScore")]
        public IActionResult ShowAllScore([FromBody]RequestTable requestTable)
        {
            (object data, string message) = IscoreService.ShowAllScore(requestTable);
            return ResponseOk(data, message);
        }
        [HttpGet("detailScore/{scoreId}")]
        public IActionResult DetailScore(string scoreId)
        {
            (ScoreReponse data, string message) = IscoreService.DetailScore(scoreId);
            if(data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data,message);
        }
        [HttpPut("editScore/{scoreId}")]
        public IActionResult EditScore(string scoreId,[FromBody] EditCreateScoreRequest valueInput)
        {
            (Score data, string message) = IscoreService.EditScore(scoreId,valueInput);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpDelete("deleteScore/{scoreId}")]
        public IActionResult DeleteScore(string scoreId)
        {
            (Score data, string message) = IscoreService.DeleteScore(scoreId);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPost("editListScore")]
        public IActionResult EditListScore([FromBody] List<EditListScoreRequest> valueInputs )
        {
            (object data, string message) = IscoreService.EditListScore(valueInputs);
            if(data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
    }
}