using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Invitations.Request
{
    public class AddTemplateRequest
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public int? Type { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
