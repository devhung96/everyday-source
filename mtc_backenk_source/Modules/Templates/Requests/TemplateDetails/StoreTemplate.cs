using Project.Modules.Templates.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TemplateDetails.Requests
{
    public class StoreTemplate
    {
        [Required]
        public string TemplateName { get; set; }

        [Required]
        public float templateRatioX { get; set; }

        [Required]
        public float templateRatioY { get; set; }
        public TEMPLATEROTATE templateRotate { get; set; } = TEMPLATEROTATE.KHONGXOAY;

        [Required]
        public List<StoreTemplateDetailValidation> templateDetails { get; set; }
    }
}
