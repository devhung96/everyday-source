using Project.Modules.Groups.Validation;
using Project.Modules.Tags.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Requests
{
    public class InsertGroupRequest
    {
        public string GroupName { get; set; }

        public string GroupCode { get; set; }

        //public List<InsertGroupDetailsRequest> GroupDetailsRequests { get; set; } = new List<InsertGroupDetailsRequest>();
    }

    public class InsertGroupDetailsRequest
    {
        [GroupExistsValidation]
        public string GroupId { get; set; }

        public DateTime? Time { get; set; }

        [ModeAuthenticationExistsValidationAttribute]
        public string ModeAuthenticationId { get; set; }
    }
}
