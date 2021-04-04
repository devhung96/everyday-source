using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Modules.UserCodes.Enities
{
    [Table("hd_bank_user_codes")]
    public class UserCode
    {
        [Key]
        [Column("user_code_id")]
        public string UserCodeId { get; set; } = Guid.NewGuid().ToString();
        [Column("user_id")]
        public string UserId { get; set; }

        [JsonIgnore]
        [Column("user_code_active")]
        public string UserCodeActive { get; set; }

        [Column("user_code_expire")]
        public DateTime? UserCodeExpire { get; set; }


        [Column("user_code_created_at")]
        public DateTime? UserCodeCreatedAt { get; set; } = DateTime.UtcNow;

    }

}
