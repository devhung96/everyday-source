using Project.Modules.Courses.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Courses.Requests
{
    public class StoreCourseRequest
    {
        [Required]
        public string CourseName { get; set; }
        [Required]
        [CourseCodeValidation]
        public string CourseCode { get; set; }
    }
}
