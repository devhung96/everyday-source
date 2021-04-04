using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class UpdateRoleRequest
    {
        public string RoleName { get; set; }
        public List<string> PermissionIds { get; set; }
    }
}
