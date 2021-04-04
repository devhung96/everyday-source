using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("mtc_user_permission")]
    public class UserPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("user_permission_id")]
        public string UserPermissionId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("permission_code")]
        public string PermissionCode { get; set; }

        [NotMapped]
        public Permission Permission { get; set; }

    }
}
