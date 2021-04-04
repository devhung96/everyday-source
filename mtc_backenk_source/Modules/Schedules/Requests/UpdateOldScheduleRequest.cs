using Project.Modules.Schedules.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class UpdateOldScheduleRequest
    {
        [Required]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd HH:mm:ss}")]
        public DateTime? ScheduleTimeBegin { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd HH:mm:ss}")]
        public DateTime? ScheduleTimeEnd { get; set; }
        public ScheduleLoopEnum ScheduleLoop { get; set; } = ScheduleLoopEnum.NOLOOP;
        public string ScheduleComment { get; set; }
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string PlaylistId { get; set; }
        public string UserId { get; set; }
        [Required]
        public ScheduleRepeatEnum ScheduleRepeat { get; set; } = ScheduleRepeatEnum.Non;
        public string ScheduleRepeatValue { get; set; }
    }
}
