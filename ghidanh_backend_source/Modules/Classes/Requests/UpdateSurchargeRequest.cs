using Project.Modules.Classes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Classes.Requests
{
    [ValidateUpdateSurchargeAttribute]
    public class UpdateSurchargeRequest
    {
        [Required]
        public string SurchargeId { get; set; }
        public string ClassId { get; set; }
        public string SurchargeName { get; set; }
        public double? SurchargeAmount { get; set; }
    }
}
