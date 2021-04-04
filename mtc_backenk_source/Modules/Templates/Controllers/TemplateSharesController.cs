using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Groups.Entities;
using Project.Modules.Templates.Entities;
using Project.Modules.Templates.Requests.TemplateShares;
using Project.Modules.Templates.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateSharesController : BaseController
    {
        private readonly ITemplateShareService TemplateShareService;

        public TemplateSharesController(ITemplateShareService templateShareService)
        {
            TemplateShareService = templateShareService;
        }

        [HttpPost]
        public IActionResult Store([FromBody] StoreTemplateShareRequest request)
        {
            var permissions = User.FindAll("Permissions").Select(x => x.Value).ToList();
            string groupId = User.FindFirst("group_id")?.Value;

            if (!permissions.Contains("TEMPLATE_SHARE"))
            {
                return ResponseBadRequest("UserDoesNotHavePermissionTEMPLATE_SHARE");
            }

            if (!request.GroupIds.Contains(groupId))
            {
                return ResponseBadRequest("GroupShareIncorrect");
            }

            (List<TemplateShare> data, string message) = TemplateShareService.Store(request);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

        [HttpGet("{templateId}")]
        public IActionResult GetGroupByTemplate([FromQuery] PaginationRequest request, string templateId)
        {
            (PaginationResponse<Group> data, string message) = TemplateShareService.GetGroupByTemplate(request, templateId);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }

        [HttpDelete("{templateId}")]
        public IActionResult Delete([FromBody] DeleteTemplateShareRequest request, string templateId)
        {
            (object data, string message) = TemplateShareService.Delete(request, templateId);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }

            return ResponseOk(data, message);
        }
    }
}
