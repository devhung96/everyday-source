using Newtonsoft.Json;
using Project.Modules.Groups.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("mtc_user_tbl")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("user_image")]
        public string UserImage { get; set; }
        [Column("user_pass")]
        [JsonIgnore]
        [DataType(DataType.Password)]
        public string UserPass { get; set; }
        [Column("user_saft")]
        [JsonIgnore]
        [DataType(DataType.Password)]
        public string UserSaft { get; set; }
        [Column("user_status")]
        [EnumDataType(typeof(UserStatusEnum))]
        public UserStatusEnum UserStatus { get; set; } = UserStatusEnum.ACTIVE;
        [Column("user_email")]
        public string UserEmail { get; set; }
        [Column("user_level")]
        [EnumDataType(typeof(UserLevelEnum))]
        public UserLevelEnum UserLevel { get; set; }
        [Column("group_id")]
        public string GroupId { get; set; }
        [Column("role_id")]
        public string RoleId { get; set; }
        [Column("user_expired_at")]
        public DateTime? ExpiredAt { get; set; }
        [Column("user_updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [Column("user_created_at")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public List<UserPermission> UserPermissions { get; set; }
        [NotMapped]
        public List<Permission> Permissions { get; set; }
        public Group Group { get; set; }
        public Role Role { get; set; }
    }
    public enum UserLevelEnum
    {
        USER = 0,
        SUPERADMIN = 1
    }
    public enum UserStatusEnum
    {
        DEACTIVE = 0,
        ACTIVE = 1,
        EXPIRED = 2
    }
}
