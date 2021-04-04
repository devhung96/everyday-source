using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Groups.Entities
{
    [Table("rc_group")]
    public class Group
    {
        [Key]
        [Column("group_code")]
        public string GroupId { get; set; }
        [Column("group_name")]
        public string GroupName { get; set; }
        [Column("group_created_at")]
        public DateTime? CreatedAt { get; set; }
        [NotMapped]
        public List<Permission> Permissions { get; set; }
        [NotMapped] 
        public List<string> PermissionCodes { get; set; }
    }
}
