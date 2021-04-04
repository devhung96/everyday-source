using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionOrganizes.Entities
{
    [Table("shareholder_function_organize")]
    public class FunctionOrganize
    {
        [Key]
        [Column("function_organize_id")]
        public int FunctionOrganizeId { get; set; }
        [Column("function_organize_name")]
        public string FunctionOrganizeName { get; set; }
        [Column("function_organize_code")]
        public string FunctionOrganizenCode { get; set; }
        [Column("module_organize_id")]
        public int ModuleOrganizeId { get; set; }
        [Column("function_organize_created_at")]
        public DateTime FunctionOrganizeCreatedAt { get; set; }
        public List<PermissionOrganize> PermissionOrganizes { get;  set; }

        public FunctionOrganize() { }
    }
}
