using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Medias.Entities
{
    [Table("hd_bank_media")]
    public class Media
    {
        [Key]
        [Column("media_id")]
        public string MediaID { get; set; } = Guid.NewGuid().ToString();
        [Column("media_name")]
        public string MediaName { get; set; }
        [Column("media_path")]
        public string MediaPath { get; set; }
        [Column("media_title")]
        public string MediaTitle { get; set; }
        [Column("media_tag")]
        public string MediaTag { get; set; }
        [Column("media_description")]
        public string MediaDescription { get; set; }
        [Column("media_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
