using Microsoft.VisualBasic;
using Project.Modules.Permissions.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Functions.Entities
{
    [Table("shareholder_function")]
    public class Function
    {
        [Key]
        [Column("function_id")]
        public int FunctionId { get; set; }
        [Column("function_name")]
        public string FunctionName { get; set; }
        [Column("function_code")]
        public string FunctionCode { get; set; }
        [ForeignKey("fk_module")]
        [Column("module_id")]
        public int ModuleId { get; set; }
        [Column("function_created_at")]
        public DateTime FunctionCreatedAt { get; set; }
        public List<Permissions.Entities.PermissionOrganize> Permissions { get; set; }
        public Function() {}
     //   public Permission Permission { get; set; }
    }
}
