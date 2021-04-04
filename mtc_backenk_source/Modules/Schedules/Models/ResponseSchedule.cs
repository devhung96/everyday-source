using Project.Modules.PlayLists.Entities;
using Project.Modules.Schedules.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Models
{
    public class ResponseSchedule
    {
        public string ScheduleId { get; set; }
        public string UserId { get; set; }
        public string PlaylistId { get; set; }
        public string ScheduleName { get; set; }
        public DateTime ScheduleDateTimeBegin { get; set; }
        public DateTime ScheduleDateTimeEnd { get; set; }
        public TimeSpan ScheduleTimeBegin { get; set; }
        public TimeSpan ScheduleTimeEnd { get; set; }
        public ScheduleRepeatEnum ScheduleRepeat { get; set; }
        public List<string> ScheduleRepeatValueData { get; set; }
        public ScheduleSequential ScheduleSequential { get; set; }
        public List<string> ScheduleRepeatValueDetailData { get; set; }
        public ScheduleLoopEnum ScheduleLoop { get; set; }
        public DateTime ScheduleCreatedAt { get; set; }
        public User User { get; set; }
        public PlayList PlayList { get; set; }
        public List<string> DeviceIds { get; set; }
    }
}
