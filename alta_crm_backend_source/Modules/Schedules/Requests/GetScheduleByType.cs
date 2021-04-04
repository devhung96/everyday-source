using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class GetScheduleByType : PaginationRequest
    {
        public int UserType { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
