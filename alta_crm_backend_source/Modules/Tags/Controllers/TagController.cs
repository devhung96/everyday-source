using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Tags.Enities;
using Project.Modules.Tags.Extensions;
using Project.Modules.Tags.Requests;
using Project.Modules.Tags.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace Project.Modules.Tags.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : BaseController
    {
        private readonly ITagService _tagService;
        private readonly IConfiguration Configuration;
        public TagController(ITagService tagServiceV2)
        {
            _tagService = tagServiceV2;

        }

        /// <summary>
        /// Hiển thị tất cả group (Có pagination) (Huỳnh Anh)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPagination([FromQuery] PaginationRequest request)
        {
            var result = _tagService.ShowAll(request);
            return ResponseOk(result);
        }

        [HttpGet("{tagId}")]
        public async Task<IActionResult> FindById(string tagId)
        {
            (object data, string message) = _tagService.ShowById(tagId);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }


        /// <summary>
        /// 0 : No Repeat 
        /// 1 : Daily 
        /// 2 : Weekly 
        /// 3 : Monthly 
        /// 4 : Yearly	
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> InsertTag([FromBody] InsertTagRequest request)
        {
            (object data, string message) = _tagService.InsertTag(request);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }

        [HttpPut("{tagId}")]
        public async Task<IActionResult> UpdateTag(string tagId, [FromBody] UpdateTagRequest request)
        {
            (object data, string message) = _tagService.UpdateTag(tagId,request);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }

        [HttpDelete("{tagId}")]
        public async Task<IActionResult> DeleteTag(string tagId)
        {
            (object data, string message) = _tagService.DeleteTag(tagId);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }
    }
}
