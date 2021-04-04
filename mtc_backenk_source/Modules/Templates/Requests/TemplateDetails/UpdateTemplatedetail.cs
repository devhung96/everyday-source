using Project.App.Request;
using Project.Modules.TemplateDetails.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TemplateDetails.Requests
{
    public class UpdateTemplatedetail : BaseRequest<TemplateDetail>
    {
        public string TemplateDetailId { get; set; }
        public string MediaId { get; set; }
        public double Zindex { get; set; }
        public float TempRatioX { get; set; }
        public float TempRatioY { get; set; }
        public double TempPointWidth { get; set; }
        public double TempPointHeight { get; set; }
        public double TempPointTop { get; set; }
        public double TempPointLeft { get; set; }
        public string TemplateId { get; set; }
    }
}
