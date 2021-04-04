using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class CreateRoleRequest
    {
        [Required]
        public string RoleName { get; set; }    
        //[Required]
        //public UserLevelEnum RoleLevel { get; set; }
        [Required]
        //[RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Accented and special characters are not allowed")] // Loại bỏ ký tự đăc biệt 
        public string RoleCode { get; set; }
        public List<string> PermissionIds { get; set; } = new List<string>();
    }
}
