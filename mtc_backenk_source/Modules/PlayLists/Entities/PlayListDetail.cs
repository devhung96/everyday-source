using Project.Modules.PlayLists.Entities;
using Project.Modules.Templates.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PlayLists.Entities
{
    [Table("mtc_playlist_details")]
    public class PlayListDetail 
    {
        [Key]
        [Column("playlistdetail_id")]
        public string PlayListDetailId { get; set; } = Guid.NewGuid().ToString();
       
        [Column("time_begin")]
        public TimeSpan TimeBegin { get; set; }
        [Column("time_end")]
        public TimeSpan TimeEnd { get; set; }
        [Column("order_media")]
        public int OrderMedia { get; set; }
        [Column("playlist_id")]
        public string PlayListId { get; set; }
        [Column("template_id")]
        public string TemplateId { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [Column("template_duration")]
        public TimeSpan? TemplateDuration { get; set; }
        public PlayList PlayList { get; set; }
        public Template Template { get; set; }



    }
    
}
