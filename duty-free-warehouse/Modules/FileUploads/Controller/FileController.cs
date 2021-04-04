using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.FileUploads.Entities;
using Project.Modules.FileUploads.Request;
using Project.Modules.FileUploads.Service;

namespace Project.Modules.FileUploads.Controller
{
    [Route("api/file")]
    [Authorize]
    [ApiController]
    public class FileController : BaseController
    {
        public readonly IFileUploadService uploadService;

        public FileController(IFileUploadService _uploadService)
        {
            uploadService = _uploadService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync([FromForm]RequestUploadFile requestCreateNew)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = uploadService.UploadFileAsync(requestCreateNew, accessToken);
            if (result.Result.data is null)
                return ResponseBadRequest(result.Result.message);
            return ResponseOk(result.Result.data, result.Result.message);
        }



        [HttpGet("{type}")]
        public IActionResult GetFile(FileTye type)
        {
            (object data, string message) = uploadService.GetAllFileWithType(type, HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value);
            if (data is null)
                return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }
    }
}