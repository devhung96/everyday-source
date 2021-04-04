using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PlayLists.Entities
{
    [Table("mtc_playlist")]
    public class PlayList
    {
        [Key]
        [Column("playlist_id")]
        public string PlayListId { get; set; } = Guid.NewGuid().ToString();
        [Column("playlist_name")]
        public string PlayListName { get; set; }
        [Column("playlist_comment")]
        public string PlayListComment { get; set; }
        [Column("playlist_loop")]
        [EnumDataType(typeof(PlayListLoopEnum))]
        public PlayListLoopEnum PlayListLoop { get; set; }
        [Column("playlist_status")]
        [EnumDataType(typeof(PlayListStatusEnum))]
        public PlayListStatusEnum PlaylistStatus { get; set; }
        [Column("playlist_assign_user_id")]
        public string PlayListAssignUserId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        

        public User User { get; set; }
        public List<PlayListDetail> PlayListDetail { get; set; }

    }
    public enum PlayListStatusEnum
    {
        SEQUENTIALLY = 1 ,
        NOTSEQUENTIALLY = 0,
        ALL = 2
    }
    
    public enum PlayListLoopEnum
    {
        LOOP = 1,
        NOLOOP = 0
    }
}
