using Project.Modules.Products.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Seals.Entity
{
    [Table("wh_seal_product")]
    public class SealProduct
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("seal_number")]
        public string SealNumber { get; set; }
        [Column("product_code")]
        public string ProductCode { get; set; }
        [Column("quantity_export")]
        public int QuantityExport { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        public Product product { get; set; }
    }
}