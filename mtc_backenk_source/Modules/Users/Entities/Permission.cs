using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("mtc_permission")]
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("permission_code")]
        public string PermissionCode { get; set; }

        [Column("permission_name")]
        public string PermissionName { get; set; }     
        [Column("module_name")]
        public string ModuleName { get; set; }

        [Column("permission_level")]
        public UserLevelEnum Level { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }

  
}
