using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Medias.Services;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogImportController : BaseController
    {
        private readonly ILogImportSevice logImportSevice;
        private readonly IMediaService mediaService;
        private readonly string BucketFaceRegister;
        private readonly string BucketFaceDetect;
        private readonly IConfiguration Configuration;
        public LogImportController(ILogImportSevice logImportSevice, IMediaService mediaService, IConfiguration configuration)
        {
            Configuration = configuration;
            this.logImportSevice = logImportSevice;
            this.mediaService = mediaService;
            BucketFaceRegister = Configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE_REGISTER"];
            BucketFaceDetect = Configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE-DETECT"];
        }
        [HttpGet("Files")]
        public IActionResult ShowFiles([FromQuery] PaginationRequest request)
        {
            (PaginationResponse<LogFile> data, string message) = logImportSevice.ShowAllFileLog(request);
            return ResponseOk(data, message);
        }
        [HttpGet("File/{logFileId}")]
        public IActionResult ShowDetailFile([FromQuery] PaginationRequest request, string logFileId)
        {
            (PaginationResponse<LogUserImport> data, string message) = logImportSevice.DataInFile(request, logFileId);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpGet("LoadImage")]
        public IActionResult LoadImage([FromQuery] LoadFile loadFile)
        {
            (byte[] byteData, string mes, object sss) = mediaService.GetFile(BucketFaceRegister, loadFile.Path).Result;
            if (byteData is null)
            {
                return ResponseNotFound();
            }
            new FileExtensionContentTypeProvider().TryGetContentType(loadFile.Path.Split("/").LastOrDefault(), out string contentType);
            return File(byteData, contentType ?? "application/octet-stream");

        }
    }
}
