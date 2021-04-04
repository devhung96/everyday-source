using Project.Modules.Destroys.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Seals.Requests
{
    public class ReportX11X12X5Request
    {
        [ValidateStringDateTime]
        public string DateFrom { get; set; }
        [ValidateStringDateTime]
        public string DateTo { get; set; }
    }
}
