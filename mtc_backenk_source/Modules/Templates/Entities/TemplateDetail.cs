using Project.Modules.Medias.Entities;
using Project.Modules.Templates.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TemplateDetails.Entities
{
    [Table("mtc_template_detail")]
    public class TemplateDetail
    {
        [Key]
        [Column("temp_detail_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TemplateDetailId { get; set; }
        [Column("template_id")]
        public string TemplateId { get; set; }
        [Column("media_id")]
        public string MediaId { get; set; }
        [Column("temp_detail_zindex")]
        public int Zindex { get; set; }
        [Column("temp_detail_ratio_x")]
        public float TempRatioX { get; set; }
        [Column("temp_detail_ratio_y")]
        public float TempRatioY { get; set; }
        [Column("temp_detail_point_width")]
        public double TempPointWidth { get; set; } //width
        [Column("temp_detail_point_height")]
        public double TempPointHeight { get; set; } //height
        [Column("temp_detail_rotate")]
        public int Temp_rotate { get; set; } = 2; // 1: xoay 2: khong xoay
        [Column("temp_detail_created_at")]
        public DateTime TemplateDetailCreatedAt { get; set; } = DateTime.UtcNow;
        public Template Template { get; set; }
        public Media Media { get; set; }
    }
}
