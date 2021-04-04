using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("wh_department_permissions")]
    public class DepartmentPermissions
    {
        [Column("department_permission_id")]
        public int ID { get; set; }
        [Column("department_permission_departmentid")]
        public int DepartmentID { get; set; }
        [Column("department_permission_permissionid")]
        public int PermissionID { get; set; }
        [Column("department_permission_departmentcode")]
        public string DepartmentCode { get; set; }
        [Column("department_permission_permissioncode")]
        public string PermissionCode { get; set; }
        public Permission Permission { get; set; }
        public Department Department { get; set; }
    }
}
