using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Products.Entities
{
    [Table("wh_menu_details")]
    public class MenuDetail
    {
        [Key]
        [Column("menu_detail_id")]
        public int MenuDetailId { get; set; }
        [Column("menu_id")]
        public int MenuId { get; set; }
        [Column("product_code")]
        public string ProductCode { get; set; }
        [Column("menu_detail_order")]
        public int MenuDetailOrder { get; set; }
        [Column("menu_detail_parlever")]
        public int MenuDetailParlever { get; set; }
        public MenuDetail() { }
        public MenuDetail(string Code) { this.ProductCode = Code; }

        public Product Product { get; set; }

    }
}
