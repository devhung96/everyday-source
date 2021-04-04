using Newtonsoft.Json.Linq;
using Project.Modules.Question.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Modules.Question.Entities
{
    [Table("shareholder_question")]
    public class Questions
    {
        [Key]
        [Column("question_id")]
        public string QuestionID { get; set; }
        [Column("session_id")]
        public string SessionID { get; set; }
        [Column("question_title")]
        public string Title { get; set; }
        [Column("question_content")]
        public string Content { get; set; }
        [Column("question_media")]
        public string Media { get; set; }
        [Column("type")]
        [EnumDataType(typeof(TypeQuestion))]
        public TypeQuestion? Type { get; set; }
        [Column("question_answer")]
        public string Answers { get; set; }
        [Column("question_feedback")]
        public string FeedBack { get; set; }
        [Column("question_answers_count")]
        public int? CountAnswers { get; set; }
        [Column("question_extends")]
        public string Extends { get; set; }
        [Column("question_option")]
        public string Option { get; set; }
        [Column("question_status")]
        [EnumDataType(typeof(StatusQuestion))]
        public StatusQuestion? Status { get; set; } = StatusQuestion.SHOW;


        [Column("created_at")]
        public DateTime CreatdAt { get; set; } = DateTime.UtcNow.AddHours(7);
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        //[JsonIgnore]
        [NotMapped]
        public JArray AnswersJson
        {
            get
            {
                if (String.IsNullOrEmpty(Answers))
                    return new JArray();
                return JArray.Parse(Answers);
            }
        }
        [NotMapped]
        public Feedback FeedbackJson
        {
            get
            {
                if (String.IsNullOrEmpty(FeedBack))
                    return null;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Feedback>(FeedBack);
            }
        }
    }
    public enum StatusQuestion
    {
        HIDDEN = 0,
        SHOW = 1,
    }
    public enum TypeQuestion
    {
        MULTICHOOSE = 1,
        QA = 2,
        CHOOSE = 3,
        RATING = 4,
        SCALE = 5
    }
}
