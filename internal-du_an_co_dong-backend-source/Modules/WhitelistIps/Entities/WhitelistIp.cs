using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.WhitelistIps.Entities
{
    [Table("shareholder_whitelistip")]
    public class WhitelistIp
    {
        [Key]
        [Column("whitelist_id")]
        public string WhitelistId { get; set; } = Guid.NewGuid().ToString();
        [Column("ip_address")]
        public string IpAddress { get; set; }
        [Column("whitelist_status")]
        public int WhitelistStatus { get; set; } = 1;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; } = null;
    }
}
