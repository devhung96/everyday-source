using Project.App.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Entities
{
    [Table("wh_products")]
    public class Product:BaseRequest<Product>
    {
        [Key]
        [Column("product_code")]
        public string ProductCode { get; set; }
        [Column("product_name")]
        public string ProductName { get; set; }
        [Column("product_unit")]
        public string ProductUnit { get; set; }
        [Column("product_type")]
        public string ProductType { get; set; }
        [Column("product_parlevel")]
        public int ProductParLevel { get; set; }
        [Column("product_status")]
        public int ProductStatus { get; set; } = 1;
        [Column("product_createdat")]
        public DateTime ProductCreatedAt { get; set; } = DateTime.Now;
        public Product() { }
        public Product(string Code, string Name, string Type, string Unit, int ParLevel) {
            this.ProductCode = Code;
            this.ProductName = Name;
            this.ProductType = Type;
            this.ProductUnit = Unit;
            this.ProductParLevel = ParLevel;
        }
        [NotMapped]
        public int IsMenu { get; set; }
    }
}
