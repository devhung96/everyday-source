using Project.Modules.Schedules.Entities;
using Project.Modules.Tags.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Requests
{
    public class UpdateTagRequest
    {
        public string TagName { get; set; }

        public string TagDescription { get; set; }
        public string TicketTypeId { get; set; }
        public string TagTimeStart { get; set; }
        public string TagTimeEnd { get; set; }

        public DateTime? TagDateStart { get; set; }
        public DateTime? TagDateEnd { get; set; }


        public ScheduleRepeatType? TagRepeat { get; set; }
        public List<string> ListRepeatValue { get; set; }
    }
}
