using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class CalendarRequest : PaginationRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <example>2021-03-01</example>
        public string DateFrom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <example>2021-03-31</example>
        public string DateTo { get; set; }
        public int DeviceStatus { get; set; }
    }
}
