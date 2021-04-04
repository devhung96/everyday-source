using System.Collections.Generic;

namespace Project.Modules.Groups.Requests
{
    public class AddPermissionGroup
    {
        public string GroupId { get; set; }
        public List<string> PermissionCodes { get; set; }
    }
}
