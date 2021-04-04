using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Models
{
    public class DeliveryReceipt
    {
        public string DeclarationNumber { get; set; }
        public string ExportDate { get; set; }
        public string ExportNumber { get; set; }
        public string DeliveryOrderFrom { get; set; }
        public string WaybillnNumber { get; set; }
        public string AcceptedAt { get; set; }
        public string FinishedAt { get; set; }
        public List<DeliveryProductReceipt> DeliveryProductReceipts { get; set; }
    }
    public class DeliveryProductReceipt
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public string DeclarationNumberImport { get; set; }
        public int? QuantityImport { get; set; }
        public int QuantityExport { get; set; }
        public int Weight { get; set; }
        public string Location { get; set; }
    }
}
