using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class GetResultSubmit
    {
        public string AppId { get; set; }
        [Required(ErrorMessage = "QuestionId không được bỏ trống.")]
        public string QuestionID { get; set; }
    }
    public class GetResultSubmitDuplicate
    {
        public string AppId { get; set; }
        public string QuestionId { get; set; }
    }
}
