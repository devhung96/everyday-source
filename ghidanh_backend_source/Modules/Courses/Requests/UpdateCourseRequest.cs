using Project.Modules.Courses.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Courses.Requests
{
    public class UpdateCourseRequest
    {
        public string CourseName { get; set; }

        public string CourseCode { get; set; }
    }
}
