using Project.Modules.Products.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Inventories.Entites
{
    [Table("wh_inventory")]
    public class Inventory
    {
        [Key]
        [Column("in_id")]
        public int InId { get; set; }
        [Column("de_number")]
        public string DeNumber { get; set; }
        [Column("in_quantity")]
        public int InQuantity { get; set; }
        [Column("product_code")]
        public string ProductCode { get; set; }

        [Column("settlement_date")]
        public Nullable<DateTime> SettlementDate { get; set; }

        public Product Product { get; set; }

    }
}
