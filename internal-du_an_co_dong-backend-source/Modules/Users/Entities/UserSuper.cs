using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Project.Modules.Organizes.Entities;

namespace Project.Modules.Users.Entities
{
    [Table("shareholder_user_super")]
    public class UserSuper
    {
        [Key]
        [Column("user_super_id")]
        public string UserSuperId { get; set; } = Guid.NewGuid().ToString();
        [Column("user_super_full_name")]
        public string FullName { get; set; }
        
        [Column("user_super_password")]
        [JsonIgnore]
        public string Password { get; set; }
        [Column("user_super_email")]
        public string Email { get; set; }
        [Column("user_super_level")]
        public USER_TYPE Level { get; set; }
        [Column("organize_id")]
        public string OrganizeId { get; set; }
        [Column("user_super_salt")]
        [JsonIgnore]
        public string Salt { get; set; }
        [Column("user_super_image")]
        public string Image { get; set; }
        [Column("user_super_created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("user_super_delete")]
        public User.DELETE_STATUS DeleteStatus { get; set; }
        public enum USER_TYPE
        {
            SUPER =1,
            ADMIN=0
        }
        public Organize Organize { get; set; }
    }
}
