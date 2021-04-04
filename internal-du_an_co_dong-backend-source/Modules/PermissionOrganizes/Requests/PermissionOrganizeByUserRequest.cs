using Project.Modules.PermissionOrganizes.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionOrganizes.Requests
{
    [CheckOrganizeUser]
    public class PermissionOrganizeByUserRequest
    {
        public string UserId { get; set; }
        public string OrganizeId { get; set; }
        public List<ModuleOrganizeItemRequest> pemissionOrganizeUsers { get; set; }
    }
}
