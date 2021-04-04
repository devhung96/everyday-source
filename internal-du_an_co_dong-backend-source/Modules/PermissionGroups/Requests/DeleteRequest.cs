using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionGroups.Requests
{
    public class DeleteRequest
    {
        public int GroupId { get; set; }
        public string PermissionCode { get; set; }
    }
}
