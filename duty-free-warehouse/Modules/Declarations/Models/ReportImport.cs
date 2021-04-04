using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Models
{
    public class ReportImport
    {
        public string DeclarationNumber { get; set; }
        public string TypeCode { get; set; }
        public string DeclaRegister { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
    }
}
