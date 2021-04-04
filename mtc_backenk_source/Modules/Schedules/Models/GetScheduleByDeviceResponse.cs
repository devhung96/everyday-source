using Project.Modules.Devices.Entities;
using Project.Modules.Schedules.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Models
{
    public class GetScheduleByDeviceResponse
    {
        public Device Device { get; set; }
        public List<Schedule> Schedules { get; set; }
    }
}
