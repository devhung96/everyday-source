using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Modes.Entities
{
    public enum ModeStatus
    {
        Enable = 1,
        Disable = 0
    }

    [Table("hdbank_ac_mode")]
    public class Mode
    {
        [Key]
        [Column("mode_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ModeId { get; set; }
        [Column("mode_name")]
        public string ModeName { get; set; }
        [Column("mode_status")]
        [EnumDataType(typeof(ModeStatus))]
        public ModeStatus ModeStatus { get; set; } = ModeStatus.Enable;
        [Column("mode_created_at")]
        public DateTime ModeCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
