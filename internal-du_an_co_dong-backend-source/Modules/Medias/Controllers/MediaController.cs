
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Project.App.Controllers;
using Project.App.Database;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using Project.Modules.Medias.Services;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Project.Modules.Medias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : BaseController
    {
        private IMediaService _mediaService;
        public readonly MariaDBContext _mariaDBContext;
        public readonly IConfiguration _configuration;
        public MediaController(IMediaService mediaService, MariaDBContext mariaDBContext, IConfiguration configuration)
        {
            _mediaService = mediaService;
            _mariaDBContext = mariaDBContext;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost("upload")]
        public IActionResult Upload([FromForm] StoredMediaRequest value)
        {
            string userId = (string)User.FindFirst("UserId").Value;
            List<Media> medias = new List<Media>();
            foreach (IFormFile mediaFile in value.MediaFiles)
            {
                Media media = _mediaService.Store(mediaFile, userId, mediaFile.ContentType, value);
                media.MediaURL = $"{_configuration["MediaService:MediaUrl"]}{media.MediaPath}{media.MediaName}";
                medias.Add(media);
            }
            return ResponseOk(medias, "FileCreateSuccess");
        }
        [Authorize]
        [HttpPut("update/{mediaID}")]
        public IActionResult Update([FromForm] UpdateInfoMediaRequest data, string mediaID)
        {
            try
            {
                string userId = (string)User.FindFirst("UserId").Value;
                Media media = _mediaService.FindID(mediaID);
                if (media is null)
                    return ResponseBadRequest("FileDoNotExists");
                if (data.MediaFile != null)
                {
                    (bool resultDeleteFile, string messageDeleteFile) = ($"{media.MediaPath}{media.MediaName}").DeleteFile();
                    string fileName = data.MediaFile.UploadFile(media.MediaPath).Result;
                    media.MediaName = fileName;
                }
                data.UserId = userId;
                media = _mediaService.Update(data, media);
                User user = _mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(userId));
                media.UserName = user == null ? null : user.FullName;
                return ResponseOk(media, "UpdateFileSuccess");
            }
            catch (Exception e)
            {
                return ResponseBadRequest(e.Message);
            }
        }
        [Authorize]
        [HttpGet("{mediaID}")]
        public IActionResult ShowID(string mediaID)
        {
            try
            {
                string userSystemID = (string)User.FindFirst("UserID").Value;
                Media media = _mediaService.FindID(mediaID);
                if (media is null)
                    return ResponseBadRequest("FileDoNotExists");
                media.MediaURL = $"{Request.Scheme}://{Request.Host.Value}{media.MediaPath}{media.MediaName}";
                return ResponseOk(media, "ShowFlieDetailSuccess");
            }
            catch (Exception e)
            {

                return ResponseBadRequest(e.Message);
            }

        }
        [Authorize]
        [HttpDelete]
        public IActionResult Delete([FromBody] ListMediaIdRequest listMediaIdRequest)
        {
            List<Media> medias = new List<Media>();
            List<string> mediaids = new List<string>();
            foreach (var MediaId in listMediaIdRequest.MediaIds)
            {
                Media media = _mediaService.FindID(MediaId);
                if (media is null)
                {
                    mediaids.Add(MediaId);
                    continue;
                }
                media.MediaURL = $"{_configuration["MediaService:MediaUrl"]}/{media.MediaPath}{media.MediaName}";
                media = _mediaService.Delete(media);
                medias.Add(media);
            }
            if (mediaids.Count > 0)
            {
                string combindedString = string.Join(",", mediaids.ToArray());
                return ResponseBadRequest("DeleteFileNoSuccessId:" + combindedString);
            }
            return ResponseOk(medias, "DeleteFileSuccess");

        }
        [Authorize]
        [HttpPost("search")]
        public IActionResult Search([FromBody] SearchRequest data)
        {
            List<Media> medias = _mediaService.Search(data);
            foreach (var media in medias)
            {
                media.MediaURL = $"{_configuration["MediaService:MediaUrl"]}{media.MediaPath}{media.MediaName}";
            }
            DataPaginationResponse pagination = new DataPaginationResponse();
            pagination.limit = data.limit;
            pagination.page = data.index;
            pagination.total = medias.Count;
            pagination.data = medias.Skip((int)pagination.page).Take((int)pagination.limit).ToArray();
            return Ok(pagination);
        }
        
    }
}