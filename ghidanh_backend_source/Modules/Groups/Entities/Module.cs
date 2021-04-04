using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Groups.Entities
{
    [Table("rc_modules")]
    public class Module
    {
        [Key]
        [Column("module_code")]
        public string ModuleCode { get; set; }
        [Column("module_name")]
        public string ModuleName { get; set; }
        [NotMapped]
        public List<Permission> Permissions { get; set; }
    }
}
