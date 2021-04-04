using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Groups.Enities;
using Project.Modules.Groups.Requests;
using Project.Modules.Groups.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : BaseController
    {
        private readonly IGroupService GroupService;
        private readonly IConfiguration Configuration;
        public GroupController(IGroupService groupService, IConfiguration configuration)
        {
            GroupService = groupService;
            Configuration = configuration;
        }

        /// <summary>
        /// Hiển thị tất cả group (Có pagination) (Huỳnh Anh)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllGroupPagination([FromQuery] PaginationRequest request)
        {
            IQueryable<Group> groups = GroupService.GetAllGroup();
            #region RequestTable
            //Search
            groups = groups.Where(m => String.IsNullOrEmpty(request.SearchContent) ||
                                       (!String.IsNullOrEmpty(m.GroupName) && m.GroupName.ToLower().Contains(request.SearchContent))
                                       )
                                ;

            //Sort
            groups = groups.ApplySort<Group>(request.OrderByQuery);
            var Pagination = PaginationHelper<Group>.ToPagedList(groups, request.PageNumber, request.PageSize);
            #endregion

            PaginationResponse<Group> paginationResponse = new PaginationResponse<Group>(Pagination, Pagination.PageInfo);
            return ResponseOk(paginationResponse, "GetGroupSuccess");
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupById(string groupId)
        {
            Group group = GroupService.GetGroupById(groupId);
            if (group is null)
            {
                return ResponseBadRequest("GroupNotExists");
            }

            return ResponseOk(group, "GetGroupByIdSuccess");
        }

        [HttpPost]
        public async Task<IActionResult> InsertGroup([FromBody] InsertGroupRequest insertGroupRequest)
        {
            Group group = new Group
            {
                GroupName = insertGroupRequest.GroupName,
                GroupCode = insertGroupRequest.GroupCode
            };

            group = GroupService.InsertGroup(group);

            //List<GroupDetail> groupDetails = new List<GroupDetail>();
            //foreach (InsertGroupDetailsRequest groupDetail in insertGroupRequest.GroupDetailsRequests)
            //{
            //    groupDetails.Add(new GroupDetail
            //    {
            //        GroupId = group.GroupId,
            //        ModeAuthenticationId = groupDetail.ModeAuthenticationId,
            //        Time = groupDetail.Time
            //    });
            //}
            //groupDetails = GroupService.InsertManyGroupDetail(groupDetails);
            //group.GroupDetails = groupDetails;
            return ResponseOk(group, "InsertGroupSuccess");
        }

        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup(string groupId, [FromBody] UpdateGroupRequest updateGroup)
        {
            Group group = GroupService.GetGroupById(groupId);
            if (group is null)
            {
                return ResponseBadRequest("GroupNotExists");
            }

            #region ValidationGroupCode
            Group checkGroupCode = GroupService.GetAllGroup(x => x.GroupCode == updateGroup.GroupCode).FirstOrDefault();
            if (checkGroupCode is not null && checkGroupCode.GroupId != group.GroupId)
            {
                return ResponseBadRequest("GroupCodeExists");
            }
            #endregion

            group.GroupName = updateGroup.GroupName ?? group.GroupName;
            group.GroupCode = updateGroup.GroupCode ?? group.GroupCode;
            group = GroupService.UpdateGroup(group);
            return ResponseOk(group, "UpdateGroupSuccess");
        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup(string groupId)
        {
            Group group = GroupService.GetGroupById(groupId);
            if (group is null)
            {
                return ResponseBadRequest("GroupNotExists");
            }

            GroupService.DeleteGroup(group);
            return ResponseOk(null, "DeleteGroupSuccess");
        }
    }
}
