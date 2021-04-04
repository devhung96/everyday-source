using Project.Modules.DeClarations.Entites;
using Project.Modules.Products.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Destroys.Entities
{
    [Table("wh_destroy_detail")]
    public class DestroyDetail
    {
        [Key]
        [Column("destroy_detail_id")]
        public int DestroyDetailId { get; set; }
        [Column("de_number")]
        public string DeClaNumber { get; set; }
       
        [Column("destroy_id")]
        public int DestroyId { get; set; }
        [Column("product_code")]
        public string ProductCode { get; set; } 
        [Column("destroy_detail_quantity")]
        public int DestroyDetailQuantity { get; set; }
        [Column("destroy_detail_note")]
        public string DestroyDetailNote { get; set; }

        [Column("product_price")]
        public double ProductPirce { get; set; }

        public Destroy Destroy { get; set; }

        public Product Product { get; set; }

        public DeClaration DeClaration { get; set; }
        [NotMapped]
        public string DeNumber 
        {
            get
            {
                return DeClaNumber;
            }
        }


    }
    
}
