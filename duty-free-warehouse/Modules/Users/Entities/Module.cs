using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("wh_modules")]
    public class Module
    {
        [Column("module_id")]
        public int ID { get; set; }
        [Column("module_name")]
        public string Name { get; set; }
        [Column("module_code")]
        public string Code { get; set; }
        [Column("module_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        [Column("module_enable")]
        public bool Enable { get; set; } = true;
        public List<Permission> Permissions { get; set; }
    }
}
