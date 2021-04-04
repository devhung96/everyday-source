using Project.App.Request;
using Project.Modules.TemplateDetails.Entities;
using Project.Modules.Templates.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TemplateDetails.Requests
{
    public class UpdateTemplateDetailVer2
    {
        public string TemplateName { get; set; }
        public float TemplateRatioX { get; set; }
        public float TemplateRatioY { get; set; }
        public TEMPLATEROTATE TemplateRotate { get; set; } = TEMPLATEROTATE.KHONGXOAY;
        public List<UpdateTemplateDetails> ListTemplateDetails { get; set; }
    }
    public class UpdateTemplateDetails
    {
        public string MediaId { get; set; }
        public int Zindex { get; set; }
        public float TempRatioX { get; set; }
        public float TempRatioY { get; set; }
        public double TempPointWidth { get; set; }
        public double TempPointHeight { get; set; }
        public string TemplateId { get; set; }
    }
}
