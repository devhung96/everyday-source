using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class DeleteScheduleRequest
    {
        [Required]
        public List<string> ScheduleIds { get; set; }
    }
}
