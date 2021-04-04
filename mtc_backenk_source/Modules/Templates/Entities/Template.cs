using Project.Modules.TemplateDetails.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Entities
{
    public enum TEMPLATESTATUS
    {
        ACTIVED = 1,
        DELETED = 0,
    }
    public enum TEMPLATEROTATE
    {
        KHONGXOAY = 2,
        XOAY = 1,
    }

    public enum TEMPLATEAPPROVED
    {
        Approved = 1,
        NotApproved = 0
    }

    public enum TemplateDefault
    {
        Default = 1,
        Normal = 0
    }

    [Table("mtc_template_tbl")]
    public class Template
    {
        [Key]
        [Column("template_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TemplateId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("template_name")]
        public string TemplateName { get; set; }
        [Column("template_ratio_x")]
        public float TemplateRatioX { get; set; }
        [Column("template_ratio_y")]
        public float TemplateRatioY { get; set; }
        [Column("template_duration")]
        public TimeSpan? TemplateDuration { get; set; }
        [Column("template_rotate")]
        public TEMPLATEROTATE TemplateRotate { get; set; } = TEMPLATEROTATE.KHONGXOAY;
        [Column("template_default")]
        public TemplateDefault TemplateDefault { get; set; } = TemplateDefault.Normal;
        public User User { get; set; }
        public List<TemplateDetail> TemplateDetails { get; set; }
        [Column("template_created_at")]
        public DateTime TemplateCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
