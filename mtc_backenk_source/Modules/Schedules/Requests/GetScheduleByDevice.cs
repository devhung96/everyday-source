using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class GetScheduleByDevice
    {
        public string DeviceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <example>2021-03-01</example>
        public string ScheduleDateBegin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <example>2021-03-31</example>
        public string ScheduleDateEnd { get; set; }
    }
}
