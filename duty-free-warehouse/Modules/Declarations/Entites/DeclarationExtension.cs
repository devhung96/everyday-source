using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Entites
{
    [Table("wh_declaration_extension")]
    public class DeclarationExtension
    {
        [Key]
        [Column("declaration_extension_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DeclarationExtensionId { get; set; }
        [Column("declaration_number")]
        public string DeclarationNumber { get; set; }
        [Column("hs_code")]
        public string HsCode { get; set; }
        [Column("product_code")]
        public string ProductCode { get; set; }
        [Column("quantity_inventory")]
        public int QuantityInventory { get; set; }
        [Column("declaration_extension_created_at")]
        public DateTime DeclarationExtensionCreatedAt { get; set; } = DateTime.Now;
    }
}
