using Project.Modules.PermissionOrganizes.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.GroupOrganizes.Requests
{
    public class AddGroupOrganizeRequest
    {
        public string GroupOrganizeName { get; set; }
        public string OrganizeId { get; set; }
        public List<ModuleOrganizeItemRequest>  PemissionGroupOrganizes { get; set; }
    }
    public class UpdateGroupOrganizeRequest
    {
        public string GroupOrganizeName { get; set; }
        public List<ModuleOrganizeItemRequest> PemissionGroupOrganizes { get; set; }
    }
}
