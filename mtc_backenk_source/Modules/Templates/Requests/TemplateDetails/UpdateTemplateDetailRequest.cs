using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TemplateDetails.Requests
{
    public class UpdateTemplateDetailRequest
    {
        [Required]
        public string TemplateName { get; set; }
        [Required]
        public List<UpdateTemplateDetailVer2> ListTemplateDetails { get; set; }
    }
}
