using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Slots.Entities
{
    [Table("rc_slots")]
    public class Slot
    {
        [Key]
        [Column("slot_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SlotId { get; set; }
        [Column("slot_name")]
        public string SlotName { get; set; }
        [Column("slot_start_at")]
        public TimeSpan SlotStartAt { get; set; }
        [Column("slot_end_at")]
        public TimeSpan SlotEndAt { get; set; }
        [Column("slot_created_at")]
        public DateTime SlotCreatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public string SlotStartAtStr { get
            {
                return SlotStartAt.ToString(@"hh\:mm");
                //return SlotStartAt.ToString();
            }
        }
        [NotMapped]
        public string SlotEndAtStr
        {
            get
            {
                return SlotEndAt.ToString(@"hh\:mm");
                //return SlotEndAt.ToString();
            }
        }
    }
}
