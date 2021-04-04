using Project.App.Request;
using Project.Modules.TemplateDetails.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TemplateDetails.Requests
{
    public class StoreTemplateDetailValidation : BaseRequest<TemplateDetail>
    {

        [Required]
        public float TempRatioX { get; set; }
        [Required]
        public float TempRatioY { get; set; }

        [Required]
        public double TempPointWidth { get; set; }
        [Required]
        public double TempPointHeight { get; set; }

        [Required]
        public int Zindex { get; set; }

        [Required]
        public string MediaId { get; set; }
        
    }
}
