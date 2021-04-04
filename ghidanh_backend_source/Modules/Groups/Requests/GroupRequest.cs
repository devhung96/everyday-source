using System.Collections.Generic;

namespace Project.Modules.Groups.Requests
{
    public class GroupRequest
    {
        public string GroupId { get; set; }
        public List<string> PermissionCodes { get; set; }
    }
}
