using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionOrganizes.Entities
{
    [Table("shareholder_permission_group_organize")]
    public class PermissionGroupOrganize
    {
        [Key]
        [Column("permission_group_organize_id")]
        public int PermissionOrganizeGroupId { get; set; }
        [Column("group_organize_id")]
        public int GroupOrganizeId { get; set; }
        [Column("permission_organize_id")]
        public int PermissionOrganizeId { get; set; }
        [Column("permission_organize_code")]
        public string PermissionOrganizeCode { get; set; }
    }
}
