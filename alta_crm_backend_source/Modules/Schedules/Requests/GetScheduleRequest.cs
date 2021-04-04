using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class GetScheduleRequest : PaginationRequest
    {
         public string DateFrom { get; set; }
         public string DateTo { get; set; }
    }
}
