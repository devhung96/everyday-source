using Project.Modules.Scores.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Requests
{
    public class EditListScoreRequest
    {
        [CheckStudentIdExistsValidation]
        [Required(ErrorMessage = "TheClassIdFieldIsRequired"), StringLength(255, ErrorMessage = "StudentIdValueCannotExceed4Characters")]
        public string StudentId { set; get; }
        [CheckClassIdExistsValidation]
        [Required(ErrorMessage = "TheClassIdFieldIsRequired"), StringLength(255, ErrorMessage = "ClassIdValueCannotExceed4Characters")]
        public string ClassId { set; get; }
        [CheckSemestersIdExistsValidation]
        [Required(ErrorMessage = "TheSemestersIdFieldIsRequired"), StringLength(255, ErrorMessage = "SemestersIdValueCannotExceed4Characters")]
        public string SemestersId { set; get; }
        public List<CourseSubjectList> CourseSubjectList { get; set; }
    }
    public class CourseSubjectList
    {
        [CheckCourseSubjectIdExistsValidation]
        [Required(ErrorMessage = "TheCourseSubjectIdFieldIsRequired"), StringLength(255, ErrorMessage = "CourseSubjectIdValueCannotExceed4Characters")]
        public string CourseSubjectId { set; get; }
        [Required(ErrorMessage = "TheScoreSubjectFieldIsRequired")]
        [CheckScoreSubjectValidation]
        public List<double?> ScoreSubject { set; get; }
    }
}
