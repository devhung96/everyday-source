﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Requests.TemplateShares
{
    public class StoreTemplateShareRequest
    {
        [Required]
        public string TemplateId { get; set; }
        [Required]
        public List<string> GroupIds { get; set; }
    }
}
