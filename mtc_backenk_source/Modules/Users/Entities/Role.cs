using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Users.Entities
{
    [Table("mtc_role_tbl")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string RoleId { get; set; }
        [Column("role_name")]
        public string RoleName { get; set; }
        [Column("role_code")]
        public string RoleCode { get; set; }    
        [Column("group_id")]
        public string GroupId { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        [Column("role_level")]
        public UserLevelEnum RoleLevel { get; set; }
        [JsonIgnore]
        public List<RolePermission> RolePermissions { get; set; }
        [NotMapped]
        public List<Permission> Permissions { get; set; } = new List<Permission>();     
      

    }
    public enum ROLE_STATUS
    {
        ACTIVED = 1,
        DELETED = 0
    }
}
