using Project.Modules.Reports.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Reports.Requests
{
    public class ExportReceipt
    {
        [Required]
        [CheckDateValidate]
        public string DateTime { get; set; }
        public DateTime DateTimeData { get; set; }
    }
}
