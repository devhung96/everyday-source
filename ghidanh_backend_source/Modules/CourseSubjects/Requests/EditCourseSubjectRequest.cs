using Project.Modules.CourseSubjects.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Requests
{
    [CheckRequiredEditValidation]
    public class EditCourseSubjectRequest
    {
        [CheckCourseIdExistsValidation]
        [Required(ErrorMessage = "TheCourseIdFieldIsRequired")]
        public string CourseId { set; get; }
        [CheckSubjectIdExistsInCourseSubjectValidation]
        [Required(ErrorMessage = "TheSubjectIdFieldIsRequired")]
        public string SubjectId { set; get; }
        [Required(ErrorMessage = "TheCheckRequiredFieldIsRequired")]
        public int CheckRequired { set; get; }
        [Required(ErrorMessage = "TheScoreTypeIdFieldIsRequired")]
        public string ScoreTypeId { set; get; }
        [Required(ErrorMessage = "ThecolumnNumberFieldIsRequired"), CheckValueColumnValidation]
        private int columnNumber { get; set; }
        public int ColumnNumber
        {
            get
            {
                if (columnNumber == 0)
                {
                    return columnNumber = 1;
                }
                return columnNumber;
            }
            set
            {
                columnNumber = value;
            }
        }
    }
    
}
