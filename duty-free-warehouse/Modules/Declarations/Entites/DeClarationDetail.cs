using Project.Modules.Products.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeClarations.Entites
{
    [Table("wh_de_details")]
    public class DeClarationDetail
    {
        [Key]
        [Column("dt_id")]
        public int DeClaDetailID { get; set; }
        [Column("de_number")]
        public string DeClaNumber { get; set; }

        [Column("product_code")]
        public string ProductCode { get; set; }
        [Column("dt_quantity")]
        public int DeClaDetailQuantity { get; set; }
        [Column("dt_invoice_price")]
        public double DeClaDetailInvoicePrice { get; set; }
        [Column("dt_invoice_value")]
        public double DeClaDetailInvoiceValue { get; set; }
        [Column("dt_product_number")]
        public string DeClaDetailProductNumber { get; set; }
        [Column("dt_own_management_code")]
        public string DeClaDetailOwnCode { get; set; }
        [Column("dt_code_re_confirm")]
        public string DeClaReconfirmCode { get; set; }
        public  DeClaration DeClaration {get;set;}

        public Product Product { get; set; }


    }
}
