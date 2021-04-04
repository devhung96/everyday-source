using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Groups.Entities;
using Project.Modules.Groups.Requests;
using Project.Modules.Groups.Sevices;

namespace Project.Modules.Groups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : BaseController
    {
        private readonly IGroupService groupService;
        public GroupController(IGroupService groupService)
        {
            this.groupService = groupService;
        }
        [HttpGet("showALl")]
        public IActionResult ShowAll()
        {
            List<Group> result = groupService.ShowAll();
            return ResponseOk(result, "Success");
        }
        [HttpPost("addPermission")]
        public IActionResult AddPermission([FromBody] AddPermissionGroup request)
        {
            (List<GroupPermission> groupPermissions, string message) = groupService.AddPermissionGroup(request);
            if (groupPermissions.Count == 0)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(groupPermissions, message);
        }
        [HttpPut("update")]
        public IActionResult UpdateGroup([FromBody] GroupRequest request)
        {
            (object group, string message) = groupService.Update(request, request.GroupId);
            if (group is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(group, message);
        }
        [HttpGet("{groupId}")]
        public IActionResult Detail(string groupId)
        {
            (Group group, string message) = groupService.Detail(groupId);
            if(group is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(group);
        }
    }
}
