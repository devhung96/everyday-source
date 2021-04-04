using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Project.Modules.Products.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Sells.Entities
{
    [Table("wh_sell_detail")]
    public class SellDetail
    {
        [Key]
        [Column("sdt_id")]
        public int ID { get; set; }
        [Column("sl_id")]
        public long SellID { get; set; }
        [Column("product_code")]
        public string ProductCode { get; set; }
        [Column("sdt_sold_number")]
        public int SoldNumber { get; set; }
        [Column("sdt_currency")]
        public string Currency { get; set; }
        [Column("sdt_price")]
        public double? Price { get; set; }
        public Product Product { get; set; }
        public Sell Sell { get; set; }
        [NotMapped]
        public double TotalSellDetail { get; set; }
    }
}
