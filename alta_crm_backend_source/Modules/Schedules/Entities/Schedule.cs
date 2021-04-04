using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Entities
{
    public enum ScheduleRepeatType
    {
        NoRepeat = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }

    [Table("hd_bank_schedules")]
    public class Schedule
    {
        [Key]
        [Column("schedule_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ScheduleId { get; set; }
        [Column("schedule_name")]
        public string ScheduleName { get; set; }
        [Column("schedule_description")]
        public string ScheduleDescription { get; set; }
        [Column("ticket_id")]
        public string TicketId { get; set; }
        [Column("tag_id")]
        public string TagId { get; set; }
        [Column("mode_id")]
        public string ModeId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("schedule_repeat_type")]
        [EnumDataType(typeof(ScheduleRepeatType))]
        public ScheduleRepeatType ScheduleRepeatType { get; set; }
        [Column("schedule_date_start")]
        public DateTime ScheduleDateStart { get; set; }
        [Column("schedule_date_end")]
        public DateTime ScheduleDateEnd { get; set; }
        [Column("schedule_time_start")]
        public TimeSpan ScheduleTimeStart { get; set; }
        [Column("schedule_time_end")]
        public TimeSpan ScheduleTimeEnd { get; set; }
        [Column("schedule_value")]
        public string ScheduleValue { get; set; }
        [Column("schedule_created_at")]
        public DateTime ScheduleCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
