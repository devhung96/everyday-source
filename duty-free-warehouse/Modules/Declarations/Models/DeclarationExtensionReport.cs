using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Models
{
    // phiếu gia hạn tờ khai (K1)
    public class DeclarationExtensionReport
    {
        public string TaxCode { get; set; }
        public string DeclarationNumber { get; set; }
        public string DeclarationDate { get; set; }
        public string DispatchNumber { get; set; }
        public string DispatchDate { get; set; }
        public string RenewalDate { get; set; }
        public string RenewalDateTo { get; set; }
        public string Note { get; set; }
        public List<HSData> HSDatas { get; set; }
    }

    public class HSData
    {
        public string HSCode { get; set; }
        public int HSImportQuantity { get; set; }
        public int HSExportQuantity { get; set; }
        public int HSInventoryQuantity { get; set; }
        public int HSRenewalQuantity { get; set; }
    }
}
