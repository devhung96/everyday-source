using Microsoft.Extensions.Configuration;
using Project.Modules.Users.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Medias.Entities
{
    [Table("shareholder_medias")]
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
        [Column("media_type")]
        public string MediaType { get; set; }
        [Column("media_alt_text")]
        public string MediaAltText { get; set; }
        [Column("media_tag")]
        public string MediaTag { get; set; }
        [Column("media_description")]
        public string MediaDescription { get; set; }
        [Column("organize_id")]
        public string OrganizeId { get; set; }
        [Column("parent_id")]
        public string ParentId { get; set; }
        [Column("media_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("user_id")]
        public string UserId { set; get; }

        [NotMapped]
        public string MediaURL { get; set; }

        public Media() { }
        public Media(string MediaName, string MediaPath,string UserId,string MediaTitle,string MediaType,string OrganizeId,string ParentId)
        {
            this.MediaName = MediaName;
            this.MediaPath = MediaPath;
            this.UserId = UserId;
            this.MediaTitle = MediaTitle;
            this.MediaType = MediaType;
            this.OrganizeId = OrganizeId;
            this.ParentId = ParentId;
        }
        [NotMapped]
        public string UserName { set; get; }
    }
}
