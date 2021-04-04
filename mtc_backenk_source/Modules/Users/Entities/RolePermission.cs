using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("mtc_role_permission_tbl")]
    public class RolePermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("role_permission_id")]
        public string RolePermissionId { get; set; }    
        [Column("role_id")]
        public string RoleId { get; set; }      
        [Column("permission_code")]
        public string PermissionCode { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
