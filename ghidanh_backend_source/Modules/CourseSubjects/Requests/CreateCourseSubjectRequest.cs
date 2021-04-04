using Project.Modules.CourseSubjects.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Requests
{
    [CheckRequiredValidation]
    public class CreateCourseSubjectRequest
    {
        [CheckCourseIdExistsValidation]
        [Required(ErrorMessage = "TheCourseIdFieldIsRequired")]
        public string CourseId { set; get; }
        [CheckSubjectIdExistsInCourseSubjectValidation]
        [Required(ErrorMessage = "TheSubjectIdFieldIsRequired")]
        public string SubjectId { set; get; }
        [CheckScoreTypeIdExistsValidation]
        [Required(ErrorMessage = "TheScoreTypeIdFieldIsRequired")]
        public string ScoreTypeId { set; get; }
        [Range(0,100, ErrorMessage = "TheFieldCheckRequiredMustBeBetween0And100")]
        [Required(ErrorMessage = "TheCheckRequiredFieldIsRequired")]
        public int CheckRequired { set; get; }
        [Range(0, 100, ErrorMessage = "TheFieldColumnNumberMustBeBetween0And100")]
        [Required(ErrorMessage = "ThecolumnNumberFieldIsRequired"),CheckValueColumnValidation]
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
