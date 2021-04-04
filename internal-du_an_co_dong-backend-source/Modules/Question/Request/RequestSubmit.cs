using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class RequestSubmit
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "AppId không được bỏ trống.")]
        public string AppId { get; set; }
        [Required(ErrorMessage = "Nội dung không được bỏ trống.")]
        public object Content { get; set; }

        /// <summary>
        /// Submit lần đầu Flag = 0
        /// Submit lần 2 (tức vote lại) = 1
        /// </summary>
        public int Flag { get; set; }
        public string ResultID { get; set; }
        public string QuestionID { get; set; }
    }
}
