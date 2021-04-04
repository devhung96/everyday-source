using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Modules.Question.Entities
{
    public class ResultAnswers
    {
        [BsonId]
        [BsonElement("result_id")]
        public string ResultId { get; set; } = GeneralHelper.generateID();
        [BsonElement("user_id")]
        public string UserId { get; set; }
        [BsonElement("result_answers")]
        public string Answers { get; set; }
        [BsonElement("stock")]
        public float? Stock { get; set; }
        [BsonElement("question_id")]
        public string QuestionID { get; set; }
        //[BsonElement("name_receiver")]
        //public string NameReceiver { get; set; }
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        //[BsonElement("survey_begin_date")]
        //public DateTime BeginDate { get; set; }
        //[BsonElement("survey_end_date")]
        //public DateTime EndDate { get; set; }
        [BsonElement("updated_at")]
        public DateTime? UpdatedAt { get; set; }


        /// <summary>
        /// 1 = Content Goc
        /// 0 = Single Answer
        /// </summary>
        [BsonElement("question")]
        public int? FullQuestion { get; set; }
        [BsonElement("authority")]
        public string Authority { get; set; }
        [NotMapped]
        public JArray AuthorityJson
        {
            get
            {
                if (String.IsNullOrEmpty(Authority))
                    return new JArray();
                return JArray.Parse(Authority);
            }
        }
        //[NotMapped]
        //public JArray ContentJson { get { return JArray.Parse(Content); } }
    }
}
