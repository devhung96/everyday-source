using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ScoreTypeController : BaseController
    {
        private readonly IScoreTypeService IscoreTypeService;
        public ScoreTypeController(IScoreTypeService _IscoreTypeService)
        {
            IscoreTypeService = _IscoreTypeService;
        }
        [HttpPost("showAllScoreType")]
        public IActionResult ShowAllScoreType([FromBody] RequestTable requestTable)
        {
            (object data, string message) = IscoreTypeService.ShowAllScoreType(requestTable);
            return ResponseOk(data,message);
        }
        [HttpPost("createScoreType")]
        public IActionResult CreateScoreType([FromBody] CreateScoreTypeRequest valueInput)
        {
            (ScoreType data, string message) = IscoreTypeService.CreateScoreType(valueInput);
            return ResponseOk(data, message);
        }
        [HttpGet("detailScoreType/{scoreTypeId}")]
        public IActionResult DetailScoreType(string scoreTypeId)
        {
            (ScoreType data, string message) = IscoreTypeService.DetailScoreType(scoreTypeId);
            if(data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpDelete("deleteScoreType/{scoreTypeId}")]
        public IActionResult DeleteScoreType(string scoreTypeId)
        {
            (ScoreType data, string message) = IscoreTypeService.DeleteScoreType(scoreTypeId);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPut("editScoreType/{scoreTypeId}")]
        public IActionResult EditScoreType([FromBody] EditScoreTypeRequest valueInput, string scoreTypeId)
        {
            (ScoreType data,List<Score> scores, string message) = IscoreTypeService.EditScoreType(scoreTypeId, valueInput);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPut("editScoreTypeV2/{scoreTypeId}")]
        public IActionResult EditScoreTypeV2([FromBody] EditScoreTypeRequest valueInput, string scoreTypeId)
        {
            (ScoreType data, List<Score> scores, string message) = IscoreTypeService.EditScoreType(scoreTypeId, valueInput);
            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(new { scoreType = data, scores = scores }, message);
        }

    }
}