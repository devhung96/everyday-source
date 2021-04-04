using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("wh_department")]
    public class Department
    {
        [Column("department_id")]
        public int ID { get; set; }
        [Column("department_code")]
        public string Code { get; set; }
        [Column("department_name")]
        public string Name { get; set; }
        [Column("department_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        [Column("department_enable")]
        public bool Enable { get; set; } = true;
        public List<User> Users { get; set; }
        public List<DepartmentPermissions> DepartmentPermissions { get; set; }
    }
}
