using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ClassSchedules.Requests
{
    public class EditScheduleRequest
    {
        [MaxLength(36)]
        public string ClassId { get; set; }
        [MaxLength(36)]
        public string SubjectId { get; set; }
        public bool? OnlineClassRoom { get; set; }
        public string ClassRoom { get; set; }
        [MaxLength(36)]
        public string LecturerId { get; set; }
        public List<string> DayOfWeek { get; set; } = new List<string>();
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string StepRepeat { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
    }
}
