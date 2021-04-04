﻿using Project.App.Request;
using Project.Modules.TemplateDetails.Entities;
using Project.Modules.TemplateDetails.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TemplateDetails.Requests
{
    public class StoreTemplateDetail : BaseRequest<TemplateDetail>
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
        [Required]
        [CheckTemplateIdValidation]
        public string TemplateId { get; set; }

    }
}
