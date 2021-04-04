using Project.Modules.Groups.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Users.Entities
{
    [Table("rc_users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("user_email")]
        public string UserEmail { get; set; }  
        [Column("user_image")]
        public string UserImage { get; set; }
        [Column("account_id")]
        public string AccountId { get; set; }
        [Column("group_code")]
        public string GroupId { get; set; }
        [Column("user_created")]
        public DateTime? CreateAt { get; set; }
        [NotMapped]
        public Group Group { get; set; }
        [NotMapped]
        public List<Permission> Permissions { get; set; }
        [NotMapped]
        public List<string> PermissionCodes { get; set; }
    }
}
