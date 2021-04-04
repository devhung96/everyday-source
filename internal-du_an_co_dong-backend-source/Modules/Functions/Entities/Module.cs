using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Functions.Entities
{
    [Table("shareholder_module")]
    public class Module
    {
        [Key]
        [Column("module_id")]
        public int ModuleId { get; set; }
        [Column("module_name")]
        public string ModuleName { get; set; }
        [Column("module_code")]
        public string ModuleCode { get; set; }
        [Column("module_created_at")]
        public DateTime ModuleCreatedAt { get; set; }
     //   public Function Function { get; set; }

        public List<Function> Functions { get; set; }
    }
}
