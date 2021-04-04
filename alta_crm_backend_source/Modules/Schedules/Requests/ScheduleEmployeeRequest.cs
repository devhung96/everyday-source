using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class ScheduleEmployeeRequest
    {
        public string UserId { get; set; }
        public string ModeId { get; set; }
        public string TagId { get; set; }
    }
}
