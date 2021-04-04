using Project.Modules.Tags.Enities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.UserTagModes.Entities
{
    [Table("hd_bank_users_tag_modes")]
    public class UserTagMode
    {
        [Key]
        [Column("users_tag_mode_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserTagModeId { get; set; }


        [Column("tag_id")]
        public string TagId { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("mode_id")]
        public string ModeId { get; set; }

        [Column("user_tag_mode_created_at")]
        public DateTime UserTagModeCreatedAt { get; set; } = DateTime.UtcNow;

        public Tag Tag { get; set; }
    }

   
   
}
