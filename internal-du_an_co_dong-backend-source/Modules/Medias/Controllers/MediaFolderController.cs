using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Database;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using Project.Modules.Medias.Services;

namespace Project.Modules.Medias.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaFolderController : BaseController
    {
        private readonly IMediaFolderService _ImediaFolderService;
        private readonly IConfiguration _configuration;
        public readonly MariaDBContext _mariaDBContext;
        public MediaFolderController(IMediaFolderService ImediaFolderService, IConfiguration configuration, MariaDBContext mariaDBContext)
        {
            _ImediaFolderService = ImediaFolderService;
            _configuration = configuration;
            _mariaDBContext = mariaDBContext;
        }
        [Authorize]
        [HttpPost("organize")]
        public IActionResult StoredFolderOrganize([FromBody] StoredFolderOrganizeRequest stored)
        {
            string userId = (string)User.FindFirst("UserId").Value;
            Media mediaFolder = _ImediaFolderService.StoredFolderOrganize(userId, stored);
            if (mediaFolder == null)
            {
                return ResponseBadRequest("MediaServer Errorr.");
            }
            mediaFolder.MediaURL = $"{_configuration["MediaService:MediaUrl"]}/{mediaFolder.MediaPath}";
            string folder = mediaFolder.MediaPath.UploadFolder(mediaFolder.MediaName);
            return ResponseOk(mediaFolder, "CreateFolderSuccess");
        }
        [HttpDelete]
        public IActionResult Delete([FromBody] ListFolderIdRequest folderIdRequest)
        {
            List<Media> mediaFolders = new List<Media>();
            List<string> mediaFolderIds = new List<string>();
            foreach (string FolderId in folderIdRequest.FolderIds)
            {
                string host = $"{Request.Scheme}://{Request.Host.Value}";
                Media mediaFolder = _ImediaFolderService.Delete(FolderId, host);
                if (mediaFolder == null)
                {
                    continue;
                }
                mediaFolders.Add(mediaFolder);
            }
            return ResponseOk(mediaFolders, "DeleteFolderSuccess");
        }
        [HttpPut("{folderId}")]
        public IActionResult Update([FromBody] UpdateFileRequest updateFileRequest, string folderId)
        {
            string host = $"{Request.Scheme}://{Request.Host.Value}";
            Media mediaFolder = _ImediaFolderService.Update(folderId, host, updateFileRequest);
            if (mediaFolder == null)
            {
                return ResponseBadRequest("FolderDoNotExists");
            }
            return ResponseOk(mediaFolder, "UpdateFolerSuccess");
        }
        [HttpGet("organize/{organizeId}")]
        public IActionResult ShowFolderOrganizeAll(string organizeId)
        {
            string host = _configuration["MediaService:MediaUrl"];
            object data = _ImediaFolderService.ShowFolderOrganizeAll(organizeId, host);
            return ResponseOk(data, "ShowAllFoldersuccess");
        }
        [HttpGet("{folderId}")]
        public IActionResult ShowDetailFolder(string folderId)
        {
            string host = $"{Request.Scheme}://{Request.Host.Value}";
            Media mediaFolder = _ImediaFolderService.FindId(folderId, host);
            return ResponseOk(mediaFolder);
        }
    }
}