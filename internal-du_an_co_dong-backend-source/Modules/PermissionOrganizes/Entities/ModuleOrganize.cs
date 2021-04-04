
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionOrganizes.Entities
{
    [Table("shareholder_module_organize")]
    public class ModuleOrganize
    {
        [Key]
        [Column("module_organize_id")]
        public int ModuleOrganizeId { get; set; }
        [Column("module_organize_name")]
        public string ModuleOrganizeName { get; set; }
        [Column("module_organize_code")]
        public string ModuleOrganizeCode { get; set; }
        [Column("module_organize_created_at")]
        public DateTime ModuleOrganizeCreatedAt { get; set; }
        public List<FunctionOrganize> FunctionOrganizes { get; set; }
    } 
}
