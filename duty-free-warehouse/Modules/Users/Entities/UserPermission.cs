using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("wh_user_permission")]
    public class UserPermission
    {
        [Column("user_permission_id")]
        public int ID { get; set; }
        [Column("user_permission_userid")]
        public int UserID { get; set; }
        [Column("user_permission_permissionid")]
        public int PermissionID { get; set; }
        [Column("user_permission_permissioncode")]
        public string PermissionCode { get; set; }
        public Permission Permission { get; set; }
        public User User { get; set; }
    }
}
