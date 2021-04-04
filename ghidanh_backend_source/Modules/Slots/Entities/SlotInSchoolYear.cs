using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Slots.Entities
{
    [Table("rc_slot_school_years")]
    public class SlotInSchoolYear
    {
        [Key]
        [Column("slot_id")]
        public string SlotId { get; set; }
        [Key]
        [Column("school_year_id")]
        public string SchoolYearId { get; set; }
        [Column("slot_school_year_create_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
