using Project.App.Request;
using Project.Modules.Templates.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Requests
{
    public class StoreTemplateRequest : BaseRequest<Template>
    {
        [Required]
        public string TemplateName { get; set; }

        [Required]
        public float TemplateRatioX { get; set; }

        [Required]
        public float TemplateRatioY { get; set; }

        public TimeSpan? TemplateDuration { get; set; }
        public TEMPLATEROTATE TemplateRotate { get; set; } = TEMPLATEROTATE.KHONGXOAY;

    }
}
