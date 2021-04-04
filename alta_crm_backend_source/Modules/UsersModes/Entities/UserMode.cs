using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.UsersModes.Entities
{
    [Table("hd_bank_users_modes")]
    public class UserMode
    {
        [Key]
        [Column("users_modes_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserModeId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("mode_id")]
        public string ModeId { get; set; }

        [Column("repository_id")]
        public string? RepositoryId { get; set; }

        [Column("users_modes_key_code")]
        public string UserModeKeyCode { get; set; }
        [Column("users_modes_image")]
        public string UserModeImage { get; set; }
        [Column("users_modes_created_at")]
        public DateTime UserModeCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
