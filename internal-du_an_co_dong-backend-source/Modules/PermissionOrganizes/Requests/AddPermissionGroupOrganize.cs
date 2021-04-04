using Project.Modules.Permissions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionOrganizes.Requests
{
    public class AddPermissionGroupOrganize
    {

        public List<ModuleOrganizeItemRequest> ModuleOrganizes { get; set; }

    }

    public class ModuleOrganizeItemRequest
    {

        public int ModuleOrganizeId { get; set; }
        public string ModuleOrganizeName { get; set; }
        public string ModuleOrganizeCode { get; set; }
        public DateTime ModuleOrganizeCreatedAt { get; set; }
        public List<FunctionOrganizeItemRequest> FunctionOrganizes { get; set; }
    }

    public class FunctionOrganizeItemRequest
    {
        public int FunctionOrganizeId { get; set; }
        public string FunctionOrganizeName { get; set; }
        public string FunctionOrganizeCode { get; set; }
        public int ModuleOrganizeId { get; set; }
        public DateTime FunctionOrganizeCreatedAt { get; set; }
        public List<PermissionOrganizeItemRequest> PermissionOrganizes { get; set; }
    }

    public class PermissionOrganizeItemRequest
    {
        public int PermissionOrganizeId { get; set; }
        public string PermissionOrganizeCode { get; set; }
        public string PermissionOrganizeName { get; set; }
        public int FunctionOrganizeId { get; set; }
        public int PermissionTypeId { get; set; }
        public PermissionType PermissionType { get; set; }
        public bool PermissionFlag { get; set; } = false;

    }

}
