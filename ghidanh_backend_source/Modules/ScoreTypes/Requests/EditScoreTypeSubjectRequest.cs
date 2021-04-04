using Project.Modules.ScoreTypes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Requests
{
    public class EditScoreTypeSubjectRequest
    {
        [CheckScoreTypeIdExistsValidation]
        [Required, StringLength(255, ErrorMessage = "ScoreTypeIdCannotExceed4Characters")]
        public string ScoreTypeId { set; get; }
        [CheckSubjectIdExistsValidation]
        [Required, StringLength(255, ErrorMessage = "SubjectIdValueCannotExceed4Characters")]
        public string SubjectId { set; get; }
        [CheckSchoolYearIdExistsValidation]
        [Required, StringLength(255, ErrorMessage = "SchoolYearIdValueCannotExceed4Characters")]
        public string SchoolYearId { set; get; }
    }
}
