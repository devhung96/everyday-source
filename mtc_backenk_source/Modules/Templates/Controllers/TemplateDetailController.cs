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
using Project.Modules.TemplateDetails.Entities;
using Project.Modules.TemplateDetails.Requests;
using Project.Modules.TemplateDetails.Services;
using Project.Modules.Templates.Entities;
using Project.Modules.Templates.Requests.TemplateDetails;
using Project.Modules.Templates.Services;

namespace Project.Modules.TemplateDetails.Controllers
{
    [Route("api/templatedetail")]
    [ApiController]
    [Authorize]
    public class TemplateDetailController : BaseController
    {
        private readonly ITemplateDetailServices TemplateDetailServices;
        private readonly ITemplateServices TemplateServices;
        private readonly IConfiguration Configuration;
        private readonly int isHttps = 0;

        public TemplateDetailController(ITemplateDetailServices templateDetailServices, ITemplateServices templateServices, IConfiguration configuration)
        {
            TemplateDetailServices = templateDetailServices;
            TemplateServices = templateServices;
            Configuration = configuration;
            isHttps = int.Parse(configuration["IsHttps"]);
        }
        [HttpPost]
        public IActionResult Store([FromBody] StoreTemplateDetail storeTemplateDetail)
        {
            (TemplateDetail templateDetail, string message) = TemplateDetailServices.Store(storeTemplateDetail);
            if (templateDetail is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(templateDetail, message);
        }

        [HttpPut("{templateDetailId}")]
        public IActionResult Update([FromBody] UpdateTemplatedetail storeTemplateDetail, string templateDetailId)
        {
            (TemplateDetail templateDetail, string message) = TemplateDetailServices.Update(storeTemplateDetail, templateDetailId);
            if (templateDetail is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(templateDetail, message);
        }

        [HttpPost("createTemplateAndDetail")]
        public IActionResult StoreTemplateAndDetail([FromBody] StoreTemplate storeTemplateDetail)
        {
            string userId = User.FindFirst("user_id")?.Value;
            (object templateDetails, string message) = TemplateDetailServices.StoreTemplateAndTemplateDetail(storeTemplateDetail, userId);
            if (templateDetails is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(templateDetails, message);
        }

        [HttpPut("updateTemplateAndDetail/{templateDetailId}")]
        public IActionResult UpdateTemplateAndDetail([FromBody] UpdateTemplateDetailVer2 updateTemplateAndDetails, string templateDetailId)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            string userId = User.FindFirst("user_id")?.Value;
            (List<TemplateDetail> templateDetails, string message) = TemplateDetailServices.UpdateRange(updateTemplateAndDetails, templateDetailId);
            if (templateDetails is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(templateDetails, "Success");
        }

        [HttpGet("{templateDetailId}")]
        public IActionResult Show(string templateDetailId)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (TemplateDetail template, string message) = TemplateDetailServices.Show(templateDetailId, urlRequest);
            if (template == null) return ResponseBadRequest(message);
            return ResponseOk(template, message);

        }

        [HttpGet]
        public IActionResult ShowByTemplateId([FromQuery] PaginationRequest requestTable)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (PaginationResponse<TemplateDetail> result, string message) = TemplateDetailServices.ShowByTemplateId(requestTable, urlRequest);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }

            return Ok(result);
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] DeleteTemplateDetailRequest request)
        {
            (bool check, string message) = TemplateDetailServices.Delete(request.TemplateIdDetails);
            if (!check)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(message);
        }
    }
}