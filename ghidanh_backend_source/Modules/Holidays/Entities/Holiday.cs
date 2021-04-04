using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Holidays.Entities
{
    [Table("rc_holidays")]
    public class Holiday
    {
        [Key]
        [Column("holiday_id")]
        public string HolidayId { get; set; } = Guid.NewGuid().ToString();

        [Column("holiday_name")]
        public string Name { get; set; } 

        [Column("holiday_timestart")]
        public DateTime TimeStart { get; set; }

        [Column("holiday_timeend")]
        public DateTime TimeEnd { get; set; }

        [Column("holiday_reason")]
        public string Reason { get; set; }

        [Column("holiday_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
