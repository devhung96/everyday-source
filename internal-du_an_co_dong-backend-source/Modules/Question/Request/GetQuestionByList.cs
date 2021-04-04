using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class GetQuestionByList
    {
        public List<string> ListQuestionId { get; set; }
        [Required(ErrorMessage = "AppId không được bỏ trống.")]
        public string AppId { get; set; }
    }
}
