using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Requests
{
    public class GetScheduleByDeviceByTimeValidation
    {
        [Required]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
        public DateTime timeStart { get; set; }
    }
}
