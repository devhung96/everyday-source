using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.PlayLists.Entities;
using Project.Modules.PlayLists.Requests;
using Project.Modules.PlayLists.Services;
//using Project.Modules.Users.Middlewares;

namespace Project.Modules.PlayLists.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[MiddlewareFilter(typeof(CheckUserExistsMiddlewareAttribute))]
    public class PlayListController : BaseController
    {
        private readonly IPlayListService _playListService;
        private readonly int isHttps = 0;
        private readonly IConfiguration configuration;
        public PlayListController(IPlayListService playListService, IConfiguration _configuration)
        {
            _playListService = playListService;
            isHttps = _configuration["IsHttps"].toInt();
        }
        [HttpPost("multiple")]
        public IActionResult StoreMultiple([FromBody] StoreMultiPlayListDetailRequest value)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (object data, string message) = _playListService.StoreMultiple(value, userId);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPost("multipleV2")]
        public IActionResult StoreMultipleV2([FromBody] StoreMultiPlayListDetailRequest value)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (object data, string message) = _playListService.StoreMultipleV2(value, userId);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpGet]
        public IActionResult showAll([FromQuery] PaginationRequest requestTable)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;
            (PaginationResponse<PlayList> data, string message) = _playListService.ShowAll(requestTable, userId, urlRequest);
            return ResponseOk(data, message);
        }
        [HttpGet("V2")]
        public IActionResult showAllV2([FromQuery] PaginationRequest requestTable)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;
            string groupId = User.FindFirst("group_id").Value;
            (PaginationResponse<PlayList> data, string message) = _playListService.ShowAllV2(requestTable, userId, urlRequest, groupId);
            return ResponseOk(data, message);
        }

        [HttpPost]
        public IActionResult store([FromBody] StorePlayListRequest value)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (PlayList data, string message) playList = _playListService.Store(value, userId);
            return ResponseOk(playList.data, playList.message);
        }
        [HttpDelete("{playListId}")]
        public IActionResult delete(string playListId)
        {
            (PlayList data, string message) playList = _playListService.Delete(playListId);
            if (playList.data == null)
                return ResponseBadRequest(playList.message);
            return ResponseOk(null, playList.message);
        }
        [HttpPut("{playListId}")]
        public IActionResult edit([FromBody] UpdatePlayListRequest valueUpdate, string playListId)
        {
            //string urlRequest = HttpContext.GetBaseURL(isHttps);
            //(PlayList playList, string message) = _playListService.FindID(playListId, urlRequest);
            //if (playList == null)
            //    return ResponseBadRequest(message);
            (PlayList result, string message) response = _playListService.EditV2(valueUpdate, playListId);
            if(response.result is null) return ResponseBadRequest(response.message);
            return ResponseOk(response.result, response.message);
        }
        [HttpPut("assign/{playListId}")]
        public IActionResult assing(string playListId, string userAssign)
        {
            (PlayList result, string message) = _playListService.AssignUser(playListId, userAssign);
            if (result == null)
                return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }

        [HttpGet("{playlistId}")]
        public IActionResult Show(string playlistId)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (PlayList playList, string message) = _playListService.FindID(playlistId, urlRequest);
            if (playList == null) return ResponseBadRequest(message);
            return ResponseOk(playList, message);
        }
        [HttpPost("copyPlayListDetail")]
        public IActionResult CopyPlayListDetail(CopyPlaylistDetailRequest valueInput)
        {
            string userId = User.FindFirst("user_id").Value.ToString();
            (bool data, string message) = _playListService.CopyPlayListDetail(valueInput, userId);
            if(data == false)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(null, message);
        }

    }
}