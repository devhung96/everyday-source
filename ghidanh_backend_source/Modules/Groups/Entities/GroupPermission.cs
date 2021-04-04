using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Entities
{
    [Table("rc_group_permission")]
    public class GroupPermission
    {
        [Key]
        [Column("group_permission_id")]
        public string GroupPermissionId { get; set; } = Guid.NewGuid().ToString();
        [Column("group_code")]
        public string GroupId { get; set; }
        [Column("permission_code")]
        public string PermissionCode { get; set; }
    }
}
