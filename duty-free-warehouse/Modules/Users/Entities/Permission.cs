using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("wh_permissions")]
    public class Permission
    {
        [Column("permission_id")]
        public int ID { get; set; }
        [Column("permission_code")]
        public string Code { get; set; }
        [Column("permission_name")]
        public string Name { get; set; }
        [Column("permission_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        [Column("permission_enable")]
        public bool Enable { get; set; } = true;
        [Column("permission_module_id")]
        public int? ModuleID { get; set; }
        public Module Module { get; set; }
        public List<DepartmentPermissions> DepartmentPermissions { get; set; }
        public List<UserPermission> UserPermissions { get; set; }
    }
}
