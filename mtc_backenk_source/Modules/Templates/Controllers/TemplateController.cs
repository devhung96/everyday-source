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
using Project.App.Middlewares;
using Project.Modules.Templates.Entities;
using Project.Modules.Templates.Requests;
using Project.Modules.Templates.Services;

namespace Project.Modules.Templates.Controllers
{
    [Route("api/template")]
    [ApiController]
    [Authorize]
    public class TemplateController : BaseController
    {
        private readonly ITemplateServices TemplateServices;
        private readonly int isHttps = 0;
        private readonly IConfiguration Configuration;
        public TemplateController(ITemplateServices templateServices, IConfiguration configuration)
        {
            TemplateServices = templateServices;
            Configuration = configuration;
            isHttps = int.Parse(configuration["IsHttps"]);
        }

        [HttpPost]
        public IActionResult Store([FromBody] StoreTemplateRequest storeTemplateRequest)
        {
            string userId = User.FindFirst("user_id")?.Value;
            (Template template, string message) = TemplateServices.Store(storeTemplateRequest, userId);
            if (template is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(template, message);
        }

        [HttpGet("{templateId}")]
        public IActionResult FindID(string templateId)
        {
            string userId = User.FindFirst("user_id")?.Value;
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (Template template, string message) = TemplateServices.FindID(templateId, userId, urlRequest);
            if (template is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(template, "Success");
        }

        [HttpGet]
        public IActionResult Show([FromQuery] PaginationRequest request)
        {
            string userId = User.FindFirst("user_id")?.Value;
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (PaginationResponse<Template> listTemplate, string message) = TemplateServices.Show(request, userId, urlRequest);
            if (listTemplate is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(listTemplate, "ShowAllTemplateSuccess");
        }

        [HttpPut("{templateId}")]
        public IActionResult Update([FromBody] UpdateTemplate updateTemplate, string templateId)
        {
            (Template template, string message) = TemplateServices.Update(updateTemplate, templateId);
            if (template is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(template, message);
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] DeleteTemplateRequest request)
        {
            (bool check, string message) = TemplateServices.Delete(request.TemplateIds);
            if (!check)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(message);
        }
    }
}