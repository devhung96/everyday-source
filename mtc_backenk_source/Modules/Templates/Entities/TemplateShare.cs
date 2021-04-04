using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Entities
{
    [Table("mtc_template_share")]
    public class TemplateShare
    {
        [Key]
        [Column("template_share_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TemplateShareId { get; set; }
        [Column("template_id")]
        public string TemplateId { get; set; }
        [Column("group_id")]
        public string GroupId { get; set; }
        [Column("user_owner")]
        public string UserCreateTemplate { get; set; }
        [Column("template_share_created_at")]
        public DateTime TemplateShareCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
