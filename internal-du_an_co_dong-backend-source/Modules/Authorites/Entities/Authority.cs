using Newtonsoft.Json;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Authorities.Entities
{
    public enum AuthorityType
    {
        EVENT = 1, // ủy quyền toàn sự kiện
        QUESTION = 2 // ủy quyền theo từng câu hỏi
    }
    public enum AuthorityStatus
    {
       PENDING = 1,
       CONFIRM = 2,
       DENY = 3
    }
    [Table("shareholder_authority")]
    public class Authority
    {
        [Key]
        [Column("authority_id")]
        public int AuthorityID { get; set; }
        [Column("authority_user_id")]
        public string AuthorityUserID { get; set; } // người không tham gia cuộc họp (trao ủy quyền)]
        [Column("authority_receive_user_id")]
        public string AuthorityReceiveUserID { get; set; } // người tham gia cuộc họp (nhận ủy quyền)
        [Column("event_id")]
        public string EventID { get; set; }
        [Column("question_id")]
        public string QuestionID { get; set; }
        [Column("authority_type")]
        [EnumDataType(typeof(AuthorityType))]
        public AuthorityType AuthorityType { get; set; }
        [Column("authority_status")]
        [EnumDataType(typeof(AuthorityStatus))]
        public AuthorityStatus AuthorityStatus { get; set; }
        [Column("authority_share")]
        public int? AuthorityShare { get; set; }
        [Column("authority_created_at")]
        public DateTime AuthorityCreatedAt { get; set; } = DateTime.Now;
        [Column("authority_updated_at")]
        public DateTime AuthorityUpdateAt { get; set; } = DateTime.Now;
        [NotMapped]
        public object AuthorityReceiveUserInfo { get; set; }
        [NotMapped]
        public object AuthorityUserInfo { get; set; }
    }
}
