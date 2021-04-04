using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class CreatePermissionRequest
    {
        [Required]
        public string PermissionName { get; set; }
        [Required]
        public string PermissionCode { get; set; }
        [Required]
        public UserLevelEnum Level { get; set; }
    }
}
