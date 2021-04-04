using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class DeleteSubmitOne
    {
        [Required(ErrorMessage = "QuestionId không được bỏ trống.")]
        public string QuestionId { get; set; }
        [Required(ErrorMessage = "UserId không được bỏ trống.")]
        public string UserId { get; set; }
    }
}
