using Project.App.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.ClassSchedules.Requests
{
    //[CheckScheduleValidation]
    public class AddScheduleRequest
    {
        [Required(ErrorMessage = "ClassIdIsRequired")]
        [MaxLength(36)]
        public string ClassId { get; set; }
        [Required(ErrorMessage = "SubjectIdIsRequired")]
        [MaxLength(36)]
        public string SubjectId { get; set; }
        [Required(ErrorMessage = "OnlineClassRoomIsRequired")]
        public bool? OnlineClassRoom { get; set; }
        [Required(ErrorMessage = "ClassRoomIsRequired")]
        public string ClassRoom { get; set; }
        [Required(ErrorMessage = "LecturerIdIsRequired")]
        [MaxLength(36)]
        public string LecturerId { get; set; }
        [Required(ErrorMessage = "DayOfWeekIsRequired")]
        [MinLength(1, ErrorMessage = "DayOfWeekNotEmpty")]
        public List<string> DayOfWeek { get; set; }
        [Required(ErrorMessage = "DateStartIsRequired")]
        public DateTime? DateStart { get; set; }
        [Required(ErrorMessage = "DateEndIsRequired")]
        public DateTime? DateEnd { get; set; }
        public string StepRepeat { get; set; }
        [Required(ErrorMessage = "TimeStartIsRequired")]
        public string TimeStart { get; set; }
        [Required(ErrorMessage = "TimeEndIsRequired")]
        public string TimeEnd { get; set; }
    }
}
