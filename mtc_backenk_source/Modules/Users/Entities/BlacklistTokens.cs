using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("mtc_blacklist_token")]
    public class BlacklistTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("blacklist_id")]
        public string BlackListID { get; set; }
        [Column("blacklist_token")]
        public string BlackListToken { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
