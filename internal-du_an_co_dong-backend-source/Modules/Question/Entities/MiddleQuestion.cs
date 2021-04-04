using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Entities
{
    [Table("shareholder_question_session")]
    public class MiddleQuestion
    {
        [Key]
        [Column("question_middle_id")]
        public string MiddleID { get; set; }
        [Column("session_id")]
        public string SessionID { get; set; }
        [Column("question_id")]
        public string QuestionID { get; set; }
        [Column("type")]
        public TypeMiddle Type { get; set; } = TypeMiddle.ACTIVE;
        [Column("before_event")]
        public BeforeEvent BeforeEvent { get; set; } = BeforeEvent.Created;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
    public enum TypeMiddle
    {
        ACTIVE = 1,
        DELETE = 0
    }
    public enum BeforeEvent
    {
        Created = 0,
        Begin = 1,
    }
}
