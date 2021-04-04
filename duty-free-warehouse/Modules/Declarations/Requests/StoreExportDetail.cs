using Project.Modules.Declarations.Validatations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class StoreExportDetail
    {
        [Required]
        [ValidateDeclaNumberImportAttribute]
        public string DeClaNumberImport { get; set; }
        [Required]
        [ValidateDeclaNumberExportAttribute]
        public string DeClaNumberExport { get; set; }
        [ValidateDateExport]
        public string DateExported { get; set; }
        public string ExportNumber { get; set; }
        public string RequestName { get; set; }
        public string ReBill { get; set; }
        public List<Detail> Details { get; set; }
    }
}
