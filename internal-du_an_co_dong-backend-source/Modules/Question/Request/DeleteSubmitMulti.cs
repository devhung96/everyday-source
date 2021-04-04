using Newtonsoft.Json.Linq;
using Project.Modules.Question.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class DeleteSubmitMulti
    {
        [Required(ErrorMessage = "QuestionId không được bỏ trống.")]
        public string QuestionId { get; set; }
        public string AppId { get; set; }
        [CheckJArray]
        public JArray ListUserId { get; set; }
        public JArray ListResult { get; set; }
    }
}
