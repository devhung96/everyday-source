using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.App.Kafka;
using Project.Modules.Tags.Entities;
using Project.Modules.Tags.Requests;
using Project.Modules.Tags.Services;
using System;

namespace Project.Modules.Tags.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : BaseController
    {
        private readonly ITagService tagService;
        private readonly KafkaProducer<string, string> producer;
        public TagsController(ITagService tagService, KafkaProducer<string, string> producer)
        {
            this.producer = producer;
            this.tagService = tagService;
        }
        [HttpGet("{tagId}")]
        public IActionResult Detail (string tagId)
        {
            (Tag tag, string message) = tagService.Detail(tagId);
            if(tag is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk (tag, message);
        }
        [HttpDelete("{tagId}")]
        public IActionResult Delete (string tagId)
        {
            (Tag tag, string message) = tagService.DeleteById(tagId);
            if (tag is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(tag, message);
        }
        [HttpPost]
        public IActionResult AddTag([FromBody] AddTagRequest request)
        {
            (Tag tag, string message) = tagService.Store(request);
            if (tag is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(tag, message);
        }
        [HttpGet]
        public IActionResult ShowTable([FromQuery] PaginationRequest request)
        {
            PaginationResponse<Tag> data = tagService.ShowTable(request);
       
            return ResponseOk(data, "Success");
        }
        [HttpPut("{tagId}")]
        public IActionResult Update([FromBody] UpdateTagRequest request,string tagId)
        {
            (object data , string message) = tagService.Update(request, tagId);
            if (data is null) return ResponseBadRequest(message);
            return ResponseOk(data, "UpdateSuccess");
        }

        [HttpGet("EmployeerInTag")]
        public IActionResult EmployeerInTag([FromQuery] UserTicketRequest request)
        {
            var data = tagService.UserTicketInTagCode(request);
            if(data is null)
            {
                return ResponseBadRequest("TagCodeIsNotExist");
            }
            return ResponseOk(data);
        }

    }
}
