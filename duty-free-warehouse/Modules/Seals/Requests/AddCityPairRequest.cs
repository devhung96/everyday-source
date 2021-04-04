using Project.App.Request;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Seals.Request
{
    public class AddCityPairRequest : BaseRequest<Citypair>
    {
        [Required]
        public string Route { get; set; }
        [Required]
        public List<Schedule> Schedule { get; set; }
        [Required]
        public CityPairStatus Status { get; set; } = CityPairStatus.DEFAULT;
        [Required]
        [CheckDateValidate]
        public string DateStart { get; set; }
        public DateTime? DateStartObj
        {
            get
            {
                DateTime.TryParseExact(DateStart, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime);
                return dateTime;
            }
        }

    }

    public class Schedule
    {
        public string departure { get; set; }
        public string arrival { get; set; }
        public string route { get; set; }
        public int typeSchedule { get; set; } = 0;
        public string flightNumber { get; set; }
        public int status { get; set; }
        public List<string> flightTime { get; set; } = new List<string>();
    }
}
