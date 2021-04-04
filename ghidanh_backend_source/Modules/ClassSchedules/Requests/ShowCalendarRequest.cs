using System;

namespace Project.Modules.ClassSchedules.Requests
{
    public class ShowCalendarRequest
    {
        public string StudentId { get; set; }
        public string LecturerId { get; set; }
        public string ClassId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
