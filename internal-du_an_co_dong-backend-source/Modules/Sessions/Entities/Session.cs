using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Sessions.Entities
{
    [Table("shareholder_session")]
    public class Session
    {
        [Key]
        [Column("session_id")]
        public string SessionId { get; set; } = Guid.NewGuid().ToString();

        [Column("session_name")]
        public string SessionName { get; set; }

        [Column("session_title")]
        public string SessionTitle { get; set; }

        [Column("session_sort")]
        public int SessionSort { get; set; }

        [Column("session_description")]
        public string SessionDescription { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }
        

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }


        [Column("event_id")]
        public string EventId { get; set; }

        [NotMapped]
        public int Total { get; set; }

        [Column("session_status")]
        [EnumDataType(typeof(SESSION_STATUS))]
        public SESSION_STATUS SessionStatus { get; set; } = SESSION_STATUS.ACTIVED;
    }

    public enum SESSION_STATUS
    {
        ACTIVED = 1,
        DELETED = 0
    }
}
