using Project.Modules.Schedules.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class StoreScheduleEmployeeRequest
    {
        public string ScheduleName { get; set; }
        public string ScheduleDescription { get; set; }
        public string TagId { get; set; }
        public string ModeId { get; set; }
        public string UserId { get; set; }
        [EnumDataType(typeof(ScheduleRepeatType))]
        public ScheduleRepeatType ScheduleRepeatType { get; set; }
        public string ScheduleDateStart { get; set; }
        public string ScheduleDateEnd { get; set; }
        public List<string> ScheduleValues { get; set; }
    }
}
