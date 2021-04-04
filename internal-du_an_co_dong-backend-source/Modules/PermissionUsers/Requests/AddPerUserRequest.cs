using Project.Modules.PermissonUsers.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissonUsers.Requests
{
    public class AddPerUserRequest
    { 
        [Required]
        [ValidateUserId]
        public string UserID { get; set; }
        [Required]
        [ValidatePermissionCode]
        public string PermissionCode { get; set; }
    }
}
