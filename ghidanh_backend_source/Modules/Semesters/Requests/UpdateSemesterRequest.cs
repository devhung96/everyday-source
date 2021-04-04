using Project.Modules.Semesters.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Semesters.Requests
{
    //[CheckInsertUpdateSemester]
    public class UpdateSemesterRequest
    {
        public string SemesterName { get; set; }
        public string SchoolYearId { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
    }
}
