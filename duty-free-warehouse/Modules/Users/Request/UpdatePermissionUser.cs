using Project.Modules.Users.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Request
{
    [ValidUpdatePermissionUser]
    public class UpdatePermissionUser
    {
        [Required]
        public Nullable<int> UserID { get; set; }
        [Required]
        public List<string> PermissionCode { get; set; }
        public List<int> PermissionID { get; set; } = new List<int>();
    }
}
