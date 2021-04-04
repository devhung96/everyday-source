using Project.Modules.Question.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Reports.Requests
{
    public class TestSubmitRequest
    {
        [Required]
        public string eventId { get; set; }
        [Required]
        public string questionId { get; set; }
        public OptionQuestion? optionQuestion { get; set; } 
    }
}
