using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionOrganizes.Entities
{
    [Table("shareholder_user_permission_organize")]
    public class UserPermissionOrganize
    {
        [Key]
        [Column("user_permission_organize_id")]
        public int UserPerOrganizeId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("organize_id")]
        public string OrganizeId { get; set; }
        [Column("permission_organize_id")]
        public int PermissionOrganizeId { get; set; }
        [Column("permission_organize_code")]
        public string PermissionOrganizeCode { get; set; }
    }
}
