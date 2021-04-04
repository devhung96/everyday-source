using Project.Modules.DeClarations.Entites;
using Project.Modules.Products.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Seals.Entity
{
    [Table("wh_seal_detail")]
    public class SealDetail
    {
        [Key]
        [Column("sealdetail_id")]
        public int SealDetailId { get; set; }
        [Column("seal_number")]
        public string SealNumber { get; set; }
        [Column("product_code")]
        public string ProductCode { get; set; }
        [Column("de_number")]
        public string DeClaNumber { get; set; }
        [Column("sealdetail_quantity_sell")]
        public int QuantitySell { get; set; } = 0;
        [Column("sealdetail_quantity_export")]
        public int QuantityExport { get; set; } = 0;
        [Column("sealdetail_quantity_inventory")]
        public int QuantityInventory { get; set; } = 0;
        [Column("sealdetail_quantity_real")]
        public int QuantityReal { get; set; } = 0;
        public Product Product { get; set; }
        public DeClaration DeClaration { get; set; } 
        public Seal Seal { get; set; }
    }
}