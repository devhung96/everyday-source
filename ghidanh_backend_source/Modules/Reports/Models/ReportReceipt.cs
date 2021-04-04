using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Reports.Models
{
    public class ReportReceipt
    {
        public string Department { get; set; }
        public string ReceiptSymbol { get; set; }
        public string NoBook { get; set; }
        public string FromReceiptNumerical { get; set; }
        public string ToReceiptNumerical { get; set; }
        public double TotalAmount { get; set; }
        public string Note { get;set; }
    }
}
