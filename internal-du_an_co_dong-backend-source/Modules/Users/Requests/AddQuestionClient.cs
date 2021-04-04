using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class AddQuestionClient
    {
        [Required]
        public string Content { get; set; }
        public string EventId { get; set; }
    }
}
