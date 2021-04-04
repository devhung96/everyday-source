using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Users.Entities.User;

namespace Project.Modules.Users.Entities
{
    [Table("shareholder_question_comment_client")]
    public class QuestionCommentClient
    {
        [Key]
        [Column("question_comment_id")]
        public string QuestionCommentId { get; set; } = Guid.NewGuid().ToString();
        [Column("question_id")]
        public string QuestionClientId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("user_name")]
        public string UserName { get; set; }
        [Column("comment_content")]
        public string QuestionContent { get; set; }
        [Column("comment_status")]
        public int QuestionStatus { get; set; } = 1;
        [Column("comment_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("comment_deleted")]
        public DateTime? DeletedAt { get; set; }

        public QuestionClient QuestionClient { get; set; }
    }
}
