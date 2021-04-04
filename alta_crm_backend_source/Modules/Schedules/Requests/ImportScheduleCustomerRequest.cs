using Microsoft.AspNetCore.Http;
using Project.Modules.Schedules.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class ImportScheduleCustomerRequest
    {
        public string ScheduleName { get; set; }
        public string ScheduleDescription { get; set; }
        public string TicketId { get; set; }
        [EnumDataType(typeof(ScheduleRepeatType))]
        public ScheduleRepeatType ScheduleRepeatType { get; set; } = ScheduleRepeatType.Daily;
        public string ScheduleDateStart { get; set; }
        public string ScheduleDateEnd { get; set; }
        public string ScheduleTimeStart { get; set; }
        public string ScheduleTimeEnd { get; set; }
        public List<string> ScheduleValues { get; set; }
        public IFormFile FileExcel { get; set; }

    }
}
