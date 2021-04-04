using Project.Modules.Permissions.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Permissions.Requests
{
    public class AddPermission
    {
        [Required]
        [ValidatePermissionCode]
        public string PermissionCode { get; set; }
        [Required]
        [ValidatePermissionName]
        public string PermissionName { get; set; }
    }
}
