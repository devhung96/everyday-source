using Project.Modules.Subjects.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Subjects.Requests
{
    public class StoreSubjectRequest
    {
        [Required]
        public string SubjectName { get; set; }
        [Required]
        [SubjectGroupIdValidation]
        public string SubjectGroupId { get; set; }
        [Required]
        [SubjectCodeValidation]
        public string SubjectCode { get; set; }
        [Required]
        [CourseIdValidation]
        public string CourseId { get; set; }
        
    }
}
