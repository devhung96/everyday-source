using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Entities
{
    [Table("mtc_device_tokens")]
    public class DeviceTokens
    {
        [Key]
        [Column("token_id")]
        public string TokenID { get; set; } = Guid.NewGuid().ToString();
        [Column("device_id")]
        public string DeviceID { get; set; }
        [Column("device_token")]
        public string DeviceToken { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
