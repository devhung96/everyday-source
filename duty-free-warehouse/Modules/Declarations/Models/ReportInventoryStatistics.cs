using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Models
{
    public class ReportInventoryStatistics
    {
        public string ImportDeclarationNumber { get; set; }
        public string ImportDeclaRegister { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductUnit { get; set; }
        public double Price { get; set; }
        public int ImportQuantity { get; set; }
        public double ImportInvoiceValue { get; set; }
        public int SellQuantity { get; set; }
        public double SellInvoiceValue { get; set; }
        public string ExportDeclarationNumber { get; set; }
        public int ExportQuantity { get; set; }
        public double ExportInvoiceValue { get; set; }
        public string ExportDeclaRegister { get; set; }
        public int DestroyQuantity { get; set; }
        public double DestroyInvoiceValue { get; set; }
        public string[] DestroyCode { get; set; }
    }
}
