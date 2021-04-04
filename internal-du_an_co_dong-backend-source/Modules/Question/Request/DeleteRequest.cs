using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class DeleteRequest
    {
        [Required(ErrorMessage = "QuestionId không được bỏ trống.")]
        public string QuestionID { get; set; }
        //[Required]
        public string AppId { get; set; }
        [Required(ErrorMessage = "SessionId không được bỏ trống.")]
        public string SessionID { get; set; }
    }
}
