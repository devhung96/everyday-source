using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Groups.Entities
{
    [Table("rc_permissions")]
    public class Permission
    {
        [Key]
        [Column("permission_code")]
        public string PermissionCode { get; set; }
        [Column("permisson_name")]
        public string PermissionName { get; set; }
        [Column("module_code")]
        public string ModuleCode { get; set; }
        [Column("permission_created")]
        public DateTime? CreatedAt { get; set; }
        [Column("permission_type")]
        public Page PermissionType { get; set; }
        public enum Page
        {
            CMS =0,
            Langding =1
        }
    }
}
