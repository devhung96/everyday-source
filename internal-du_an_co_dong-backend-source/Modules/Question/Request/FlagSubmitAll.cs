﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class FlagSubmitAll
    {
        //[Required(ErrorMessage = "EventId không được bỏ trống.")]
        public string EventId { get; set; }
        [Required(ErrorMessage = "QuestionId không được bỏ trống.")]
        public string QuestionId { get; set; }
        public string AppId { get; set; }
    }
}
