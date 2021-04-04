using Project.Modules.Schedules.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class StoreScheduleRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <example>2021-03-01 00:00:01</example>
        public string ScheduleDateTimeBeginStr { get; set; } //  validate
        /// <summary>
        /// 
        /// </summary>
        /// <example>2021-03-31 23:59:59</example>
        public string ScheduleDateTimeEndStr { get; set; } //  validate
        public DateTime ScheduleDateTimeBegin { get; set; }
        public DateTime ScheduleDateTimeEnd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <example>08:00:00</example>
        public string ScheduleTimeBeginStr { get; set; } //  validate
        /// <summary>
        /// 
        /// </summary>
        /// <example>12:00:00</example>
        public string ScheduleTimeEndStr { get; set; } //  validate
        public TimeSpan ScheduleTimeBegin { get; set; }
        public TimeSpan ScheduleTimeEnd { get; set; }
        public ScheduleLoopEnum ScheduleLoop { get; set; }
        public List<string> DeviceIds { get; set; }
        public string PlaylistId { get; set; }
        public string UserId { get; set; }
        public ScheduleRepeatEnum ScheduleRepeat { get; set; }
        public List<string> ScheduleRepeatValues { get; set; } //  validate
        public string ScheduleRepeatValue { get; set; }
        public string ScheduleRepeatValueDetail { get; set; }
        public List<string> ScheduleRepeatValueDetails { get; set; } // validate
    }
}
