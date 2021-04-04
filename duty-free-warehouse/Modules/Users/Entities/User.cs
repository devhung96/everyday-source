using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("wh_user")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [JsonIgnore]
        [Column("user_password")]
        public string Password { get; set; }
        [JsonIgnore]
        [Column("user_salt")]
        public string Salt { get; set; }
        [Column("user_full_name")]
        public string FullName { get; set; }
        [Column("user_department_id")]
        public int DepartmentID { get; set; }
        [Column("user_enable")]
        public bool Enable { get; set; } = true;
        [Column("user_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<UserPermission> UserPermissions { get; set; }
        public Department Department { get; set; }

    }
}
