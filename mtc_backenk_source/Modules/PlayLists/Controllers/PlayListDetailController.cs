using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Medias.Validations;
using Project.Modules.PlayLists.Entities;
using Project.Modules.PlayLists.Requests;
using Project.Modules.PlayLists.Services;
using Project.Modules.Users.Middlewares;

namespace Project.Modules.PlayListDetails.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    //[MiddlewareFilter(typeof(CheckUserExistsMiddlewareAttribute))]
    public class PlayListDetailController : BaseController
    {
        private readonly IPlayListDetailService _playListDetailService;
        public PlayListDetailController(IPlayListDetailService playListDetailService)
        {
            _playListDetailService = playListDetailService;
        }
        [HttpGet]
        public IActionResult ShowAll([FromQuery] PaginationRequest requestTable)
        {
            (IQueryable<PlayListDetail> data, string message) = _playListDetailService.ShowAll();
            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.SearchContent) ||
                (!string.IsNullOrEmpty(requestTable.SearchContent) &&
                    (
                       (x.Template != null && x.Template.TemplateName.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                       (x.PlayList != null && x.PlayList.PlayListName.ToLower().Contains(requestTable.SearchContent.ToLower()))
                    )
                ));

            PaginationHelper<PlayListDetail> playListDetailInfo = PaginationHelper<PlayListDetail>.ToPagedList(data, requestTable.PageNumber, requestTable.PageSize);
            PaginationResponse<PlayListDetail> paginationResponse = new PaginationResponse<PlayListDetail>(playListDetailInfo, playListDetailInfo.PageInfo);
            #endregion
            return ResponseOk(paginationResponse,"ShowAllSuccess");
        }
        [HttpPost]
        public IActionResult Store([FromBody] StorePlayListDetailRequest value)
        {
            (PlayListDetail data, string message) playPlistDetail = _playListDetailService.Store(value);
            if (playPlistDetail.data == null)
                return ResponseBadRequest(playPlistDetail.message);
            return ResponseOk(playPlistDetail.data, playPlistDetail.message);
        }
        [HttpPut("{playListDetailId}")]
        public IActionResult Edit(string playListDetailId, UpdatePlayListDetailRequest valueUpdate)
        {
            (PlayListDetail value, string message) playListDetail = _playListDetailService.FindID(playListDetailId);
            if (playListDetail.value == null)
                return ResponseBadRequest(playListDetail.message);
            (PlayListDetail data, string message) playListDetailResponse = _playListDetailService.Edit(playListDetail.value, valueUpdate);
            if (playListDetailResponse.data is null)
            {
                return ResponseBadRequest(playListDetailResponse.message);
            }
            return ResponseOk(playListDetailResponse.data, playListDetailResponse.message);
        }
        [HttpDelete("{playListDetailId}")]
        public IActionResult Delete(string playListDetailId)
        {
            (PlayListDetail data, string message) playListDetail = _playListDetailService.Delete(playListDetailId);
            if (playListDetail.data == null)
                return ResponseBadRequest(playListDetail.message);
            return ResponseOk(playListDetail.data, playListDetail.message);
        }

        [HttpGet("show-by-playlist-id/{playListId}")]
        public IActionResult ShowAllPlayListDetailGroup(string playListId, [FromQuery] PaginationRequest requestTable)
        {
            (IQueryable<PlayListDetail> data, string message) = _playListDetailService.ShowByPlaylistId(playListId);
            if (data == null)
                return ResponseBadRequest(message);
            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.SearchContent) ||
                (!string.IsNullOrEmpty(requestTable.SearchContent) &&
                    (
                       (x.PlayList != null && x.PlayList.PlayListName.ToLower().Contains(requestTable.SearchContent.ToLower())) ||
                       (x.Template != null && x.Template.TemplateName.ToLower().Contains(requestTable.SearchContent.ToLower()))
                    )
                ));
            PaginationHelper<PlayListDetail> playListDetailInfo = PaginationHelper<PlayListDetail>.ToPagedList(data, requestTable.PageNumber, requestTable.PageSize);
            PaginationResponse<PlayListDetail> paginationResponse = new PaginationResponse<PlayListDetail>(playListDetailInfo, playListDetailInfo.PageInfo);
            #endregion
            return ResponseOk(paginationResponse, "ShowAllSuccess");
        }

        [HttpPut("order/{playListDetailId1}/{playListDetailId2}")]
        public IActionResult EditOrder(string playListDetailId1, string playListDetailId2)
        {
            (PlayListDetail data, string message) playListDetail = _playListDetailService.EditOrder(playListDetailId1, playListDetailId2);
            if (playListDetail.data == null)
                return ResponseBadRequest(playListDetail.message);
            return ResponseOk(playListDetail.data, playListDetail.message);
        }

        [HttpGet("{idPlaylistDetailId}")]
        public IActionResult Show(string idPlaylistDetailId)
        {
            (PlayListDetail playListDetail, string message) = _playListDetailService.FindID(idPlaylistDetailId);
            if (playListDetail == null) return ResponseBadRequest(message);
            return ResponseOk(playListDetail, message);
        }


    }
}