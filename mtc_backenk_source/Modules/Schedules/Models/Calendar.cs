using Project.Modules.Devices.Entities;
using Project.Modules.PlayLists.Entities;
using Project.Modules.Schedules.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Models
{
    public class Calendar
    {
        public Device Device { get; set; }
        public List<ScheduleItem> ScheduleItems { get; set; }
    }

    public class ScheduleItem
    {
        public string ScheduleId { get; set; }
        public User User { get; set; }
        public PlayList PlayList { get; set; }
        public DateTime ScheduleDateTimeBegin { get; set; }
        public DateTime ScheduleDateTimeEnd { get; set; }
        public string ScheduleTimeBegin { get; set; }
        public string ScheduleTimeEnd { get; set; }
        public ScheduleRepeatEnum ScheduleRepeat { get; set; }
        public List<string> ScheduleRepeatValues { get; set; }
        public List<string> ScheduleRepeatValueDetails { get; set; }
        public ScheduleLoopEnum ScheduleLoop { get; set; }
        public DateTime ScheduleCreatedAt { get; set; }
    }

}
