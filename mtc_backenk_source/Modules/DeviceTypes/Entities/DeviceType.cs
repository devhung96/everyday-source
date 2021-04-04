using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeviceTypes.Entities
{
    [Table("mtc_device_type")]
    public class DeviceType
    {
        [Key]
        [Column("type_id")]
        public string DeviceTypeId { get; set; } = Guid.NewGuid().ToString();

        [Column("type_name")]
        public string DeviceTypeName { get; set; }

        [Column("type_icon")]
        public string DeviceTypeIcon { get; set; }

        [Column("type_comment")]
        public string DeviceTypeComment { get; set; }

        [Column("playlist_default")]
        public int? PlaylistDefault { get; set; }

        [Column("type_status")]
        public DeviceTypeStatus TypeStatus { get; set; } = DeviceTypeStatus.ACTIVED;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } 
    }
    public enum DeviceTypeStatus
    {
        DEACTIVATED = 2,
        ACTIVED = 1,
        DELETED = 0
    }
}
