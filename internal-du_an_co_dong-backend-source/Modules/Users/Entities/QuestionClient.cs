using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Project.Modules.Events.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Users.Entities.User;

namespace Project.Modules.Users.Entities
{
    [Table("shareholder_question_client")]
    public class QuestionClient
    {
        [Key]
        [Column("question_id")]
        public string QuestionId { get; set; } = Guid.NewGuid().ToString();
        [Column("event_id")]
        public string EventId { get; set; }
        [Column("session_id")]
        public string SessionId { get; set; } = "";
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("question_content")]
        public string QuestionContent { get; set; }
        [Column("question_status")]
        public QUESTION_STATUS QuestionStatus { get; set; } = QUESTION_STATUS.NEW;
        [Column("question_show")]
        public bool QuestionActive { get; set; } = false;
        [Column("question_created_at")]
        public DateTime QuestionCreatedAt { get; set; } = DateTime.Now;
        [Column("question_deleted")]
        public DateTime? QuestionDeleted { get; set; }

        public Event Event { get; set; }
        public User User { get; set; }
        public List<QuestionCommentClient> QuestionCommentClient { get; set; }
    }

    public class QuestionClientSocket
    {
        public string questionId { get; set; }
        public string questionContent { get; set; }
        public DateTime questionCreatedAt { get; set; }

        public string userName { get; set; }
        public string comment { get; set; }
        public string userComment { get; set; }
    }

    public enum QUESTION_STATUS
    {
        DISMISS = -1, // từ chối
        NEW = 0, // mới
        ALLOW = 1, // chấp nhận  (Active)
        ANSWERED = 2, // đã trả lời
    }
}
