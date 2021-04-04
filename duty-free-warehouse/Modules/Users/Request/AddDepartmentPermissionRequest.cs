using Project.Modules.Users.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Request
{
    [ValidAddPermissionAndDepartment]
    public class AddDepartmentPermissionRequest
    {
        [Required]
        public List<string> permissioncode { get; set; }
        [Required]
        public string departmentcode { get; set; }

        public List<int> permissionid { get; set; } = new List<int>();
        public int departmentid { get; set; }

    }
}
