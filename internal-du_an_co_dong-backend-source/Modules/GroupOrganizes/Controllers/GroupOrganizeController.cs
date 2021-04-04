using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.GroupOrganizes.Entities;
using Project.Modules.GroupOrganizes.Requests;
using Project.Modules.GroupOrganizes.Services;

namespace Project.Modules.GroupOrganizes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupOrganizeController : BaseController
    {
        private readonly IGroupOrganizeService groupOrganizeService;
        public GroupOrganizeController (IGroupOrganizeService groupOrganizeService)
        {
            this.groupOrganizeService = groupOrganizeService;
        }
        [HttpGet("show-by-group-organize/{groupOrganizeId}")]
        public IActionResult ShowAllPermissionByGroupOrganize(int groupOrganizeId)
        {
            var result = groupOrganizeService.ShowAllPermissonByGroupOrganize(groupOrganizeId);
            if (result is null)
            {
                return ResponseBadRequest("GroupNotExist");
            }
            return ResponseOk(result);
        }

        [HttpPost("add-group-organize")]
        public IActionResult AddGroupOrganize([FromBody] AddGroupOrganizeRequest request)
        {
            string userId = "4031f7ff-5283-4c0b-a30c-e46fbe22ffe9";
            (GroupOrganize groupOrganize, string message) = groupOrganizeService.AddGroupOrganize(request, userId);
            if(groupOrganize is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(groupOrganize, message);
        }
        
        [HttpPut("update-group-organize/{groupOrganizeId}")]
        public IActionResult UpdateGroupOrganize([FromBody] UpdateGroupOrganizeRequest request, int groupOrganizeId )
        {
            (GroupOrganize groupOrganize, string message) = groupOrganizeService.UpdateGroupOrganize( groupOrganizeId,request);
            if (groupOrganize is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(groupOrganize, message);
        }
        [HttpDelete("delete-group-organize/{groupOrganizeId}")]
        public IActionResult DeleteGroupOrganize(int groupOrganizeId)
        {
            (GroupOrganize groupOrganize, string message) = groupOrganizeService.DeleteGroupOrganize(groupOrganizeId);
            if(groupOrganize is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(groupOrganize, message);
        }
    }
}
