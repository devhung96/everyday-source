using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Groups.Entities;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using Project.Modules.Medias.Services;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[MiddlewareFilter(typeof(CheckUserExistsMiddlewareAttribute))]
    public class MediaController : BaseController
    {
        private readonly IMediaService MediaService;
        private readonly IMediaSupport _mediaSupport;
        private readonly IMapper Mapper;
        private readonly int isHttps = 0;
        public MediaController(IMediaService mediaService, IMapper mapper, IMediaSupport mediaSupport)
        {
            MediaService = mediaService;
            _mediaSupport = mediaSupport;
            Mapper = mapper;
            isHttps = 1;
        }

        [HttpGet("mediaOnYear")]
        public IActionResult ListMediaOnYear()
        {
            return ResponseOk(MediaService.ListMediaOnYear());
        }

        [HttpGet]
        public IActionResult ShowAll([FromQuery] ListMediaRequest request)
        {
            string url = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }

            DateTime? date = null;
            if (DateTime.TryParseExact(request.Date, "yyyy_MM", null, System.Globalization.DateTimeStyles.None, out DateTime dateParse))
            {
                date = dateParse;
            }

            List<string> typeMedias = new();
            if (!string.IsNullOrEmpty(request.TypeMedia))
            {
                typeMedias = request.TypeMedia.Split(",").ToList();
            }

            IQueryable<Media> medias = MediaService.ListMedia()
                .Where(x => x.UserId == userId || x.MediaGroups.Any(v => v.GroupId.Equals(user.GroupId)))
                .Where(x => typeMedias.Count == 0 || (
                    (typeMedias.Contains("image") && x.MediaTypeCode.Contains("image")) ||
                    (typeMedias.Contains("audio") && x.MediaTypeCode.Contains("audio")) ||
                    (typeMedias.Contains("video") && x.MediaTypeCode.Contains("video"))
                ))
                .OrderByDescending(x => x.CreateAt);

            if (request.IsConfirm != null)
            {
                if (request.IsConfirm.Value)
                {
                    medias = medias.Where(x => x.MediaStatus == MediaStatusEnum.Confirm);
                }
                else
                {
                    medias = medias.Where(x => x.MediaStatus != MediaStatusEnum.Confirm);
                }
            }
            if (request.Status != null)
            {
                medias = medias.Where(x => x.MediaStatus == request.Status);
            }

            if (date != null)
            {
                medias = medias.Where(x => x.CreateAt.Month == date.Value.Month && x.CreateAt.Year == date.Value.Year);
            }

            if (!string.IsNullOrEmpty(request.SearchContent))
            {
                medias = medias.Where(x => x.MediaName.Contains(request.SearchContent) || x.MediaComment.Contains(request.SearchContent));
            }

            medias = SortHelper<Media>.ApplySort(medias, request.OrderByQuery);

            PaginationHelper<Media> pagination = PaginationHelper<Media>.ToPagedList(medias, request.PageNumber, request.PageSize);
            foreach (var item in pagination)
            {
                if (item.TypeId != "Link")
                {
                    item.MediaUrl = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(item.MediaUrl));
                }
                if (!string.IsNullOrEmpty(item.MediaUrlOptional))
                {
                    item.MediaUrlOptional = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(item.MediaUrlOptional));
                }
            }
            PaginationResponse<Media> response = new PaginationResponse<Media>(pagination, pagination.PageInfo);
            return ResponseOk(response);
        }

        [HttpGet("v2")]
        public IActionResult ShowAllVersion2([FromQuery] ListMediaVersion2Request request)
        {
            string url = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }


            DateTime? date = null;
            if (DateTime.TryParseExact(request.Date, "yyyy_MM", null, System.Globalization.DateTimeStyles.None, out DateTime dateParse))
            {
                date = dateParse;
            }

            List<string> typeMedias = new();
            if (!string.IsNullOrEmpty(request.TypeMedia))
            {
                typeMedias = request.TypeMedia.Split(",").ToList();
            }

            List<MediaStatusEnum> mediaStatusEnums = new List<MediaStatusEnum>();
            if (!string.IsNullOrEmpty(request.Status))
            {
                mediaStatusEnums = request.Status.Split(",").Select(x => (MediaStatusEnum)Enum.Parse(typeof(MediaStatusEnum), x)).ToList();
            }

            IQueryable<Media> medias = MediaService.ListMedia()
                .Where(x => x.UserId == userId || x.MediaGroups.Any(v => v.GroupId.Equals(user.GroupId)))
                .Where(x => typeMedias.Count == 0 || (
                    (typeMedias.Contains("image") && x.MediaTypeCode.Contains("image")) ||
                    (typeMedias.Contains("audio") && x.MediaTypeCode.Contains("audio")) ||
                    (typeMedias.Contains("video") && x.MediaTypeCode.Contains("video"))
                ))
                .Where(x => mediaStatusEnums.Contains(x.MediaStatus) || String.IsNullOrEmpty(request.Status))
                .OrderByDescending(x => x.CreateAt);



            if (date != null)
            {
                medias = medias.Where(x => x.CreateAt.Month == date.Value.Month && x.CreateAt.Year == date.Value.Year);
            }

            if (!string.IsNullOrEmpty(request.SearchContent))
            {
                medias = medias.Where(x => x.MediaName.Contains(request.SearchContent) || x.MediaComment.Contains(request.SearchContent));
            }


            medias = SortHelper<Media>.ApplySort(medias, request.OrderByQuery);
            PaginationHelper<Media> pagination = PaginationHelper<Media>.ToPagedList(medias, request.PageNumber, request.PageSize);
            foreach (var item in pagination)
            {
                if (item.TypeId != "Link")
                {
                    item.MediaUrl = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(item.MediaUrl));
                }

                if (!string.IsNullOrEmpty(item.MediaUrlOptional))
                {
                    item.MediaUrlOptional = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(item.MediaUrlOptional));
                }
            }
            PaginationResponse<Media> response = new PaginationResponse<Media>(pagination, pagination.PageInfo);
            return ResponseOk(response);
        }

        [HttpGet("{mediaId}")]
        public IActionResult DetailMedia(string mediaId)
        {
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }

            Media media = MediaService.DetailMedia(mediaId);
            if (media is null)
            {
                return ResponseBadRequest("MediaNotFound");
            }

            if (media.MediaGroups.Any(x => x.GroupId != user.GroupId) && media.UserId != userId)
            {
                return ResponseBadRequest("UserNotHavePermissionForThisMedia");
            }

            string url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
            //media.MediaUrl = GeneralHelper.UrlCombine(url, media.MediaUrl);
            if (media.TypeId != "Link")
            {
                media.MediaUrl = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(media.MediaUrl));
            }
            if (!string.IsNullOrEmpty(media.MediaUrlOptional))
            {
                media.MediaUrlOptional = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(media.MediaUrlOptional));
            }

            return ResponseOk(media, "ShowDetailSuccess");
        }

        [HttpPost]
        [RequestSizeLimit(long.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> StoreAsync([FromForm] AddMediaRequest request)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;

            Media newMedia = new Media
            {
                MediaName = request.MediaName,
                TypeId = !string.IsNullOrEmpty(request.MediaType) ? request.MediaType : "Default",
                MediaComment = request.MediaComment,
                UserId = userId,
                MediaExtend = request.MediaExtend
            };

            if (request.MediaUrl != null)
            {
                (string videoPath, string typeFile) = await GeneralHelper.UploadFileOld(request.MediaUrl, "uploads/medias");
                newMedia.MediaUrl = GeneralHelper.UrlCombine("uploads/medias", videoPath);
                newMedia.MediaTypeCode = typeFile;
            }

            if (request.MediaUrlOptional != null)
            {
                (string videoPath, string typeFile) = await GeneralHelper.UploadFileOld(request.MediaUrlOptional, "uploads/mediasOption");
                newMedia.MediaUrlOptional = GeneralHelper.UrlCombine("uploads/mediasOption", videoPath);
            }

            Media media = MediaService.Store(newMedia, urlRequest);

            #region Assign group
            List<string> groupIds = new List<string>();
            if (!string.IsNullOrEmpty(request.GroupIds))
            {
                groupIds = JsonConvert.DeserializeObject<List<string>>(request.GroupIds);
            }
            List<MediaGroup> mediaGroups = new List<MediaGroup>();
            foreach (var item in groupIds)
            {
                Group group = MediaService.DetailGroupUser(item);
                if (group is null)
                {
                    return ResponseBadRequest("GroupUserNotFound");
                }

                mediaGroups.Add(new MediaGroup
                {
                    GroupId = item,
                    MediaId = media.MediaId,
                });
            }
            MediaService.DeleteGroupMediaOld(media);

            MediaService.UpdateListMediaGroup(mediaGroups);
            #endregion

            media.MediaUrl = null;
            return ResponseOk(media, "AddMediaSuccess");
        }

        [HttpPost("addLink")]
        public IActionResult StoreAddLink([FromBody] AddLinkMediaRequest input)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;
            Media media = new Media
            {
                MediaName = input.MediaName,
                TypeId = "Link",
                MediaUrl = input.MediaUrl,
                MediaComment = input.MediaComment,
                UserId = userId,
                MediaTypeCode = "link",
                MediaExtend = input.MediaExtend
            };

            if (String.IsNullOrEmpty(media.MediaName))
            {
                media.MediaName = GeneralHelper.GetFileNameWithoutExtension(media.MediaUrl);
            }
            media = MediaService.Store(media, urlRequest);
            #region Assign group
            List<string> groupIds = new List<string>();
            if (!string.IsNullOrEmpty(input.GroupIds))
            {
                groupIds = JsonConvert.DeserializeObject<List<string>>(input.GroupIds);
            }
            List<MediaGroup> mediaGroups = new List<MediaGroup>();
            foreach (var item in groupIds)
            {
                Group group = MediaService.DetailGroupUser(item);
                if (group is null)
                {
                    return ResponseBadRequest("GroupUserNotFound");
                }

                mediaGroups.Add(new MediaGroup
                {
                    GroupId = item,
                    MediaId = media.MediaId,
                });
            }
            MediaService.DeleteGroupMediaOld(media);

            MediaService.UpdateListMediaGroup(mediaGroups);
            #endregion
            return ResponseOk(media, "AddLinkSuccess");
        }

        [HttpPut("{mediaId}/confirm")]
        public IActionResult ConfirmMedia(string mediaId)
        {
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }

            Media media = MediaService.DetailMedia(mediaId);
            if (media is null)
            {
                return ResponseBadRequest("MediaNotFound");
            }

            if (media.MediaGroups.Any(x => x.GroupId != user.GroupId) && media.UserId != userId)
            {
                return ResponseBadRequest("UserNotHavePermissionForThisMedia");
            }

            if (media.MediaStatus == MediaStatusEnum.NotConfirm)
            {
                media.MediaStatus = MediaStatusEnum.Confirm;
                media.UpdatedAt = DateTime.Now;
            }

            (Media data, string message) = MediaService.Update(media);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

        [HttpPut("confirm")]
        public IActionResult ConfirmMedia([FromBody] List<string> mediaIds)
        {
            foreach (var item in mediaIds)
            {
                Media media = MediaService.DetailMedia(item);
                if (media is null)
                {
                    return ResponseBadRequest("MediaNotFound");
                }
                media.MediaStatus = MediaStatusEnum.Confirm;
                (Media data, string message) = MediaService.Update(media);
                if (data is null)
                {
                    return ResponseBadRequest(message);
                }
            }
            return ResponseOk(null, "ConfirmMediaSuccess");
        }

        [HttpPut("reject")]
        public IActionResult RejectMedia([FromBody] List<string> mediaIds)
        {
            foreach (var item in mediaIds)
            {
                Media media = MediaService.DetailMedia(item);
                if (media is null)
                {
                    return ResponseBadRequest("MediaNotFound");
                }
                media.MediaStatus = MediaStatusEnum.Reject;
                (Media data, string message) = MediaService.Update(media);
                if (data is null)
                {
                    return ResponseBadRequest(message);
                }
            }
            return ResponseOk(null, "RejectMediaSuccess");
        }

        [HttpPut("{mediaId}")]
        public IActionResult Update([FromBody] UpdateMediaRequest request, string mediaId)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }

            Media media = MediaService.DetailMedia(mediaId);
            if (media is null)
            {
                return ResponseBadRequest("MediaNotFound");
            }

            if (media.MediaGroups.Any(x => x.GroupId != user.GroupId) && media.UserId != userId)
            {
                return ResponseBadRequest("UserNotHavePermissionForThisMedia");
            }

            media = Mapper.Map(request, media);
            if(!string.IsNullOrEmpty(media.MediaExtend))
            {
                media.MediaExtend = request.MediaExtend;
            }    
            media.UpdatedAt = DateTime.Now;

            (Media data, string message) = MediaService.Update(media);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            if (data.TypeId != "Link")
            {
                data.MediaUrl = GeneralHelper.UrlCombine(urlRequest, GeneralHelper.GetURLVideo(data.MediaUrl));
            }

            return ResponseOk(data, message);
        }

        [HttpPut("{mediaId}/files")]
        public async Task<IActionResult> UpdateAsync([FromForm] UpdateMediaRequest request, string mediaId)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }

            Media media = MediaService.DetailMedia(mediaId);
            if (media is null)
            {
                return ResponseBadRequest("MediaNotFound");
            }

            if (media.MediaGroups.Any(x => x.GroupId != user.GroupId) && media.UserId != userId)
            {
                return ResponseBadRequest("UserNotHavePermissionForThisMedia");
            }

            media = Mapper.Map(request, media);
            if (!string.IsNullOrEmpty(media.MediaExtend))
            {
                media.MediaExtend = request.MediaExtend;
            }
            media.UpdatedAt = DateTime.Now;

            if (request.MediaUrlOption != null)
            {
                if (media.MediaUrlOptional != null)
                {
                    try
                    {
                        GeneralHelper.DeleteFile(media.MediaUrlOptional);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("File Not Found");
                    }
                }

                (string videoPath, _) = await GeneralHelper.UploadFileOld(request.MediaUrlOption, "uploads/mediasOption");
                media.MediaUrlOptional = GeneralHelper.UrlCombine("uploads/mediasOption", videoPath);
            }


            (Media data, string message) = MediaService.Update(media);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            if (data.TypeId != "Link")
            {
                data.MediaUrl = GeneralHelper.UrlCombine(urlRequest, GeneralHelper.GetURLVideo(data.MediaUrl));
            }

            return ResponseOk(data, message);
        }

        [HttpDelete("deleteMulti")]
        public IActionResult DeleteMuityAsync([FromBody] List<string> mediaIds)
        {
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }

            string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;

            foreach (var item in mediaIds)
            {
                Media media = MediaService.DetailMedia(item);
                if (media is null)
                {
                    return ResponseBadRequest("MediaNotFound");
                }

                if (media.MediaGroups.Any(x => x.GroupId != user.GroupId) && media.UserId != userId)
                {
                    return ResponseBadRequest("UserNotHavePermissionForThisMedia");
                }

                MediaService.Destroy(media, urlRequest);
            }

            return ResponseOk(null, "DeleteMediaSuccess");
        }

        [HttpDelete("{mediaId}")]
        public IActionResult DeleteAsync(string mediaId)
        {
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }

            string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
            Media media = MediaService.DetailMedia(mediaId);
            if (media is null)
            {
                return ResponseBadRequest("MediaNotFound");
            }

            if (media.MediaGroups.Any(x => x.GroupId != user.GroupId) && media.UserId != userId)
            {
                return ResponseBadRequest("UserNotHavePermissionForThisMedia");
            }

            MediaService.Destroy(media, urlRequest);
            return ResponseOk(null, "DeleteMediaSuccess");
        }

        [HttpGet("{mediaId}/groups")]
        public IActionResult GetGroupInMedia(string mediaId)
        {
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }

            Media media = MediaService.DetailMedia(mediaId);
            if (media is null)
            {
                return ResponseBadRequest("MediaNotFound");
            }

            if (media.MediaGroups.Any(x => x.GroupId != user.GroupId) && media.UserId != userId)
            {
                return ResponseBadRequest("UserNotHavePermissionForThisMedia");
            }

            object a = MediaService.GetListMediaGroup(media);
            return ResponseOk(a);
        }

        [HttpPut("{mediaId}/groups")]
        public IActionResult UpdateGroupInMedia(string mediaId, [FromBody] List<string> groupIds)
        {
            string userId = User.FindFirst("user_id").Value;
            User user = MediaService.DetailUser(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserNotFound");
            }

            Media media = MediaService.DetailMedia(mediaId);
            if (media is null)
            {
                return ResponseBadRequest("MediaNotFound");
            }

            if (media.MediaGroups.Any(x => x.GroupId != user.GroupId) && media.UserId != userId)
            {
                return ResponseBadRequest("UserNotHavePermissionForThisMedia");
            }

            List<MediaGroup> mediaGroups = new List<MediaGroup>();
            foreach (var item in groupIds)
            {
                Group group = MediaService.DetailGroupUser(item);
                if (group is null)
                {
                    return ResponseBadRequest("GroupUserNotFound");
                }

                mediaGroups.Add(new MediaGroup
                {
                    GroupId = item,
                    MediaId = mediaId,
                });
            }

            MediaService.DeleteGroupMediaOld(media);
            MediaService.UpdateListMediaGroup(mediaGroups);
            return ResponseOk(null, "AssignGroupMediaSuccess");
        }
        /// <summary>
        /// Luyên Thêm Hỗ trợ Vcpmc
        /// </summary>
        [HttpPost("getList")]
        public IActionResult GetList([FromBody] ListMediaIdRequest request)
        {
            var mediaFulls = MediaService.ListMedia();
            List<Media> medias = mediaFulls.Where(x => request.MediaIds.Contains(x.MediaId)).ToList();
            if (medias.Count < request.MediaIds.Distinct().Count())
            {
                return ResponseBadRequest("ListMediaInValid");
            }
            return ResponseOk(medias, "Success");
        }



        /// <summary>
        /// Kiểm tra media có free không.
        /// 1: Free (Duyệt rồi)
        /// 0: Không free (Chưa duyệt)
        /// -1: Không free (Từ chối)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ByMediaIds")]
        public IActionResult GetMediaByIds([FromBody] GetMediaByIdsRequest request)
        {
            var data = _mediaSupport.GetMediaByListIds(request);
            return ResponseOk(data);
        }

        /// <summary>
        /// Check media in group 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CheckMediaInGroup")]
        public IActionResult CheckMediaInGroup([FromBody] CheckMediaInGroupRequest request)
        {
            var data = _mediaSupport.CheckMediaInGroup(request);
            return ResponseOk(data);
        }

        /// <summary>
        /// Share medias into group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ShareIntoGroup")]
        public IActionResult ShareIntoGroup([FromBody] ShareIntoGroupRequest request)
        {
            (var data, string message) = _mediaSupport.ShareMediaIntoGroup(request);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }

        /// <summary>
        /// Share media into multiple group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ShareMutipleIntoGropup")]
        public IActionResult ShareMultipleIntoGroup([FromBody] ShareMultipleIntoGroupRequest request)
        {
            (var data, string message) = _mediaSupport.ShareMultipleIntoGroup(request);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }
        /// <summary>
        /// Share list media to all group unexpired 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost("ShareMediasAllGroup")]
        public IActionResult ShareMutipAllGroup([FromBody] ShareMediasAllGroupRequest request)
        {
            (var data, string message) = _mediaSupport.ShareListMediasAllGroup(request);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }
        [HttpPost("ApproveMedias")]
        public IActionResult ApproveMedias([FromBody] ShareMediasAllGroupRequest request)
        {
            (var data, string message) = _mediaSupport.ApproveMedias(request);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }
    }
}