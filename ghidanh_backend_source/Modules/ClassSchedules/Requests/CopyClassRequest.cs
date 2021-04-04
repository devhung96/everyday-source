using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.ClassSchedules.Requests
{
    public class CopyClassRequest
    {
        [Required(ErrorMessage = "CourseNameIsRequired")]
        [MaxLength(255)]
        public string CourseName { get; set; }
        [Required(ErrorMessage = "CourseCodeIsRequired")]
        [MaxLength(255)]
        public string CourseCode { get; set; }
        [Required(ErrorMessage = "DateStartIsRequired")]
        public DateTime? DateStart { get; set; }
        [Required(ErrorMessage = "DateEndIsRequired")]
        public DateTime? DateEnd { get; set; }
        [Required(ErrorMessage = "CourseIdCopyIsRequired")]
        [MaxLength(36)]
        public string CourseIdCopy { get; set; }
    }
}
