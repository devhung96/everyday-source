using Project.Modules.Holidays.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Holidays.Requests
{
    [CheckTimeValidation]
    public class UpdateHolidayRequest
    {
        public string HolidayName { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string Reason { get; set; }
    }
}
