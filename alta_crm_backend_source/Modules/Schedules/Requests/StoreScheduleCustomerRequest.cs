using Project.Modules.Schedules.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class StoreScheduleCustomerRequest
    {
        public string ScheduleName { get; set; }
        public string ScheduleDescription { get; set; }
        public string TicketId { get; set; }
        public string ModeId { get; set; }
        public string UserEmail { get; set; }
        [EnumDataType(typeof(ScheduleRepeatType))]
        public ScheduleRepeatType ScheduleRepeatType { get; set; }
        public string ScheduleDateStart { get; set; }
        public string ScheduleDateEnd { get; set; }
        public string ScheduleTimeStart { get; set; }
        public string ScheduleTimeEnd { get; set; }
        public List<string> ScheduleValues { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPhone { get; set; }
        public string UserAddress { get; set; }
        public int UserGender { get; set; }
    }
}
