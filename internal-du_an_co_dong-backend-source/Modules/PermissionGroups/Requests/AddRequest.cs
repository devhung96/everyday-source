using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionGroups.Requests
{
    public class AddRequest
    {
        public int GroupId { get; set; }
        public List<int> PermissionIds { get; set; }
        
    }
}
