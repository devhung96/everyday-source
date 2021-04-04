using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Entities
{
    public class AverageScore
    {
        [BsonElement("averge_score_id"), BsonId]
        public string AvergeScoreId { set; get; } = Guid.NewGuid().ToString();
        [BsonElement("student_id")]
        public string StudentId { set; get; }
        [BsonElement("class_id")]
        public string ClassId { set; get; }

        [BsonElement("subject_id")]
        public string SubjectId { set; get; }


        [Column("semester_id")]
        public string SemesterId { get; set; }

        [BsonElement("averge_score_value")]
        public double? AvergeScoreValue { set; get; }



    }

    public enum AVERGE_SCORE_STATUS
    {
        CLOSED = 2,
        OPENING = 1
    }
}
