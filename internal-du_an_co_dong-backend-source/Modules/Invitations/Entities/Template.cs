using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Invitations.Entities
{
    [Table("shareholder_template")]
    public class Template
    {
        [Key]
        [Column("template_id")]
        public string TemplateId { get; set; } = Guid.NewGuid().ToString();
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("event_id")]
        public string EventId { get; set; }
        [Column("template_title")]
        public string TemplateTitle { get; set; }
        [Column("template_type")]
        public int TemplateType { get; set; }
        [Column("template_type_name")]
        public string TemplateTypeName { get; set; } = "";
        [Column("template_content")]
        public string TemplateContent { get; set; }
        [Column("template_status")]
        public TEMPLATE_STATUS TemplateStatus { get; set; } = TEMPLATE_STATUS.ACTIVE;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public User User { get; set; }
    }

    public enum TEMPLATE_STATUS
    {
        ACTIVE = 1,
        IN_ACTIVE = 0
    }
}
