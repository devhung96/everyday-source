using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Models
{
    public class WarehouseReceipt
    {
        public string ReceivingDate { get; set; }
        public string RcvOrderNo { get; set; }
        public string Supplier { get; set; }
        public string DeliverersName { get; set; }
        public string AcceptedAt { get; set; }
        public string FinishedAt { get; set; }
        public string DeclarationNumber { get; set; }
        public int? CTNS { get; set; }
        public string TotalCTNS => CTNS?.ToString() + " PKGS";
        public string Weight { get; set; }
        public string TotalWeight => Weight + " kg";
        public List<WarehouseProductReceipt> ProductReceipts { get; set; }
    }
    public class WarehouseProductReceipt
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int TotalCTNS { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public int Weight { get; set; }
        public string Location { get; set; }
        public string Note { get; set; }
    }
}
