using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Requests.TemplateDetails
{
    public class DeleteTemplateDetailRequest
    {
        [Required]
        public List<string> TemplateIdDetails { get; set; }
    }
}
