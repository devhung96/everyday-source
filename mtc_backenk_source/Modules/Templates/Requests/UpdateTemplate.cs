using Project.App.Request;
using Project.Modules.Templates.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Requests
{
    public class UpdateTemplate : BaseRequest<Template>
    {
        public string TemplateName { get; set; }
        public float TemplateRatioX { get; set; }
        public float TemplateRatioY { get; set; }
        public TimeSpan? TemplateDuration { get; set; }
        public TEMPLATEROTATE TemplateRotate { get; set; } = TEMPLATEROTATE.KHONGXOAY;
    }
}
