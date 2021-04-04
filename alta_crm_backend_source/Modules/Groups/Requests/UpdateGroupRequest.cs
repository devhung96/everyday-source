using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Requests
{
    public class UpdateGroupRequest
    {
        public string GroupName { get; set; }

        public string GroupCode { get; set; }

        //public List<UpdateGroupDetailRequest> GroupDetailsRequests { get; set; } = new List<UpdateGroupDetailRequest>();
    }
    public class UpdateGroupDetailRequest : InsertGroupDetailsRequest
    {
        public string GroupDetailId { get; set; }
    }
}
