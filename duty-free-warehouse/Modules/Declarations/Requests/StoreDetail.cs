using Project.Modules.Declarations.Validatations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class StoreDetail
    {
        [Required]
        public string DeClaNumber { get; set; }
        public List<Detail> Details { get; set; }
        public string ImportNumber { get; set; }
        public string Supplier { get; set; }
        public string Deliver { get; set; }
        [ValidateDateAdded]
        public string DateAdded { get; set; }

    }
    public class Detail
    {
        [Required]
        public string ProductCode { get; set; }
        [Required]
        public int DeClaDetailQuantity { get; set; }
        public double DeClaDetailInvoicePrice { get; set; }
        public double? DeClaDetailInvoiceValue { get; set; }
        public string DeClaDetailProductNumber { get; set; }
        public string DeClaDetailOwnCode { get; set; }
        public string DeClaReconfirmCode { get; set; }
    }
}
