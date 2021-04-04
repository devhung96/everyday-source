using Project.App.Helpers;
using Project.App.Validations;
using Project.Modules.Subjects.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Subjects.Requests
{
    [ValidateRequestTable]
    public class SearchSubjectRequest : RequestTable
    {
        [SubjectGroupIdValidation]
        public string SubjectGroupId { get; set; }
        [CourseIdValidation]
        public string CourceId { get; set; }
    }
}
