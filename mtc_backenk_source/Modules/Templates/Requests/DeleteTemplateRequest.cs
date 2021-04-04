using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Requests
{
    public class DeleteTemplateRequest
    {
        [Required]
        public List<string> TemplateIds { get; set; }
    }
}
