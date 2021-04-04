using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Project.App.Controllers;
using Project.Modules.Aws.Requests;
using Project.Modules.Aws.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Project.Modules.Aws.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AwsController : BaseController
    {
        private readonly IAwsService UploadFileService;
        public AwsController(IAwsService uploadFileService)
        {
            UploadFileService = uploadFileService;
        }

        /// <summary>
        /// ### Effect -- Upload hình lên S3
        /// ### Artist -- An
        /// ### Des -- Backend Test
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadFile([FromForm] UploadFileRequest request)
        {
            var upload = UploadFileService.Upload("face-detect-register", request.File, request.FolderPath).Result;
            if (!upload.check)
            {
                return ResponseBadRequest("An error occurred");
            }
            return ResponseOk(new { Path = request.FolderPath, FileName = request.File.FileName }, "Success");
        }

        /// <summary>
        /// ### Effect -- Get hình ở S3
        /// ### Artist -- An
        /// ### Des -- Backend Test
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetFile([FromQuery] GetFileRequest request)
        {
            var getFile = UploadFileService.GetFile(request.FileFullName).Result;
            if (getFile.data is null)
            {
                return ResponseBadRequest(getFile.message);
            }
            new FileExtensionContentTypeProvider().TryGetContentType(request.FileFullName, out string contentType);
            return File((byte[])getFile.data, contentType ?? "application/octet-stream");
        }

        /// <summary>
        /// ### Effect -- Delete hình ở S3
        /// ### Artist -- An
        /// ### Des -- Backend Test
        /// </summary>
        /// /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete([FromQuery] DeleteFileRequest request)
        {
            var result = UploadFileService.DeleteFile(request.Key).Result;
            if (!result.check)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.message);
        }
    }
}
