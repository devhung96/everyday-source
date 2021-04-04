using Newtonsoft.Json;
using Project.Modules.Devices.Entities;
using Project.Modules.PlayLists.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Entities
{
    public enum ScheduleStatusEnum
    {
        DELETED = 0,
        USING = 1
    }

    public enum ScheduleLoopEnum
    {
        LOOP = 1,
        NOLOOP = 0
    }

    public enum ScheduleRepeatEnum
    {
        Non = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }

    public enum ScheduleSequential
    {
        On = 1,
        Off = 0
    }

    [Table("mtc_schedule_tbl")]
    public class Schedule
    {
        [Key]
        [Column("schedule_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ScheduleId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("playlist_id")]
        public string PlaylistId { get; set; }
        [Column("schedule_name")]
        public string ScheduleName { get; set; }
        [Column("schedule_datetime_begin")]
        public DateTime ScheduleDateTimeBegin { get; set; }
        [Column("schedule_datetime_end")]
        public DateTime ScheduleDateTimeEnd { get; set; }
        [Column("schedule_time_begin")]
        public TimeSpan ScheduleTimeBegin { get; set; }
        [Column("schedule_time_end")]
        public TimeSpan ScheduleTimeEnd { get; set; }
        [Column("schedule_repeat")]
        [EnumDataType(typeof(ScheduleRepeatEnum))]
        public ScheduleRepeatEnum ScheduleRepeat { get; set; } = ScheduleRepeatEnum.Non;
        /// <summary>
        /// ScheduleRepeatValue has data when ScheduleRepeat != Non and Daily
        /// Example : 
        ///     + Weekly : Mon , Tue , Wed ... Sun
        ///     + Monthly : 01 , 02, 03 ... 31
        ///     + Yearly : 01/01 , 02/01 , 12/12 ....
        /// Effect : Show customers what they have chosen (client use)
        /// </summary>
        [Column("schedule_repeat_value")]
        public string ScheduleRepeatValue { get; set; }
        [Column("schedule_sequential")]
        [EnumDataType(typeof(ScheduleSequential))]
        public ScheduleSequential ScheduleSequential { get; set; } = ScheduleSequential.Off;
        [NotMapped]
        public List<string> ScheduleRepeatValueData
        {
            get
            {
                if (!string.IsNullOrEmpty(ScheduleRepeatValue))
                {
                    return JsonConvert.DeserializeObject<List<string>>(ScheduleRepeatValue);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// ScheduleRepeatValueDetail has data when ScheduleRepeat != Non and Daily
        /// Example Data : 2021-03-02 (yyyy-MM-dd)
        /// Effect : Detailed data to get and compare date schedule (server use)
        /// </summary>
        [Column("schedule_repeat_value_detail")]
        public string ScheduleRepeatValueDetail { get; set; }
        [NotMapped]
        public List<string> ScheduleRepeatValueDetailData
        {
            get
            {
                if (!string.IsNullOrEmpty(ScheduleRepeatValueDetail))
                {
                    return JsonConvert.DeserializeObject<List<string>>(ScheduleRepeatValueDetail);
                }
                else
                {
                    return null;
                }
            }
        }
        [Column("schedule_loop")]
        [EnumDataType(typeof(ScheduleLoopEnum))]
        public ScheduleLoopEnum ScheduleLoop { get; set; }
        [Column("schedule_created_at")]
        public DateTime ScheduleCreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        [NotMapped]
        public Device Device { get; set; }
        public PlayList PlayList { get; set; }
        public Schedule() { }
    }
}
