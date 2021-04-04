using Project.Modules.Functions.Entities;
using Project.Modules.Permissions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Functions.Requests
{
    public class AddPermissonToUserRequest
    {
        public List<ModuleItemRequest> Modules { get; set; }
    }


    public class ModuleItemRequest
    {

        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleCode { get; set; }
        public DateTime ModuleCreatedAt { get; set; }
        public List<FunctionItemRequest> Functions { get; set; }
    }

    public class FunctionItemRequest
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public string FunctionCode { get; set; }
        public int ModuleId { get; set; }
        public DateTime FunctionCreatedAt { get; set; }
        public List<PermissionItemRequest> Permissions { get; set; }
    }

    public class PermissionItemRequest
    {
        public int PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string PermissionName { get; set; }
        public int FunctionId { get; set; }
        public int PermissionTypeId { get; set; }
        public PermissionType PermissionType { get; set; }
        public bool PermissionFlag { get; set; } = false;

    }

}
