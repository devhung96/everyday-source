using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class UpdatePosition
    {
        //[Required(ErrorMessage ="QuestionId không được bỏ trống.")]
        public string QuestionIdOne { get; set; }
        //[Required(ErrorMessage ="Question không được bỏ trống.")]
        public string QuestionIdTwo { get; set; }
        public string AppId { get; set; }
        public List<QuestionSort> ListQuestion { get; set; }
    }
    public class QuestionSort
    {
        [Required(ErrorMessage = "QuestionId không được bỏ trống.")]
        public int Order { get; set; }
        [Required(ErrorMessage = "QuestionId không được bỏ trống.")]
        public string QuestionId { get; set; }
    }
}
