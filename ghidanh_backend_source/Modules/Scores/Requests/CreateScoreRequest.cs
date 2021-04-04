using Project.Modules.CourseSubjects.Validations;
using Project.Modules.Scores.Validations;
using Project.Modules.ScoreTypes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Requests
{
    [CheckScoreNumberValidation,CheckStudentInClassValidation]
    public class CreateScoreRequest
    {
        [CheckCourseSubjectIdExistsValidation]
        [Required(ErrorMessage = "TheCourseSubjectIdFieldIsRequired"), StringLength(255, ErrorMessage = "CourseSubjectIdValueCannotExceed255Characters")]
        public string CourseSubjectId { set; get; }
        [CheckStudentIdExistsValidation]
        [Required(ErrorMessage = "TheStudentIdFieldIsRequired"), StringLength(255, ErrorMessage = "StudentIdValueCannotExceed255Characters")]
        public string StudentId { set; get; }
        [CheckClassIdExistsValidation]
        [Required(ErrorMessage = "TheClassIdFieldIsRequired"), StringLength(255, ErrorMessage = "ClassIdValueCannotExceed255Characters")]
        public string ClassId { set; get; }
        [CheckSemestersIdExistsValidation]
        [Required(ErrorMessage = "TheSemestersIdFieldIsRequired"), StringLength(255, ErrorMessage = "SemestersIdValueCannotExceed255Characters")]
        public string SemestersId { set; get; }
        [Required(ErrorMessage = "TheScoreSubjectFieldIsRequired")]
        [CheckScoreSubjectValidation]
        public List<double?> ScoreSubject { set; get; }
    }
}
