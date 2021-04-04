using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using static Project.Modules.Groups.Entities.Group;

namespace Project.Modules.Groups.Requests
{
    public class UpdateGroupRequest : GroupRequest
    {
    }
    public class StoreGroupRequest : GroupRequest
    {

        public DateTime? Expired { get; set; }
    }
    public class ListGroupRequest
    {
        public List<string> GroupIds { get; set; }
    }

    public class GroupRequest
    {
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public IFormFile ImageGroup { get; set; }
    }
}
