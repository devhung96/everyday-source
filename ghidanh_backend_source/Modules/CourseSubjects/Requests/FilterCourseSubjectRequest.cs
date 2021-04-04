using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Requests
{
    public class FilterCourseSubjectRequest
    {
        public string CourseId { get; set; }
        public string SubjectId { get; set; }
    }
}
