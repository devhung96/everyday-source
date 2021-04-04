using Project.Modules.Subjects.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Subjects.Requests
{
    public class UpdateSubjectRequest
    {
        //[SubjectCodeValidation]
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        [CourseIdValidation]
        public string CourseId { get; set; }
        [SubjectGroupIdValidation]
        public string SubjectGroupId { get; set; }
    }
}
