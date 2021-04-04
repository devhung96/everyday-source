using Project.Modules.Groups.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Medias.Entities
{
    [Table("mtc_media_group")]
    public class MediaGroup
    {
        [Key]
        [Column("media_group_id")]
        public string MediaGroupId { get; set; } = Guid.NewGuid().ToString();
        [Column("group_id")]
        public string GroupId { get; set; }
        [Column("media_id")]
        public string MediaId { get; set; }
        [Column("media_group_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("media_group_updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public Media Media { get; set; }
        public Group Group { get; set; }

    }
}