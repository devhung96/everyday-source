using Project.Modules.Users.Valdations;
using System.Collections.Generic;

namespace Project.Modules.Users.Requests
{
    public class UpdatePermissionRequest
    {
        [ValidateUserId]
        public string UserId { get; set; }
        [ValidationPermissionCodes]
        public List<string> PermissionCodes { get; set; }
    }
}
