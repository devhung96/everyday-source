using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Project.Modules
{
    public class Score
    {
        [BsonElement("score_id"), BsonId]
        public string ScoreId { set; get; } = Guid.NewGuid().ToString();
        [BsonElement("student_id")]
        public string StudentId { set; get; }
        [BsonElement("class_id")]
        public string ClassId { set; get; }
        [BsonElement("semesters_id")]
        public string SemestersId { set; get; }
        [BsonElement("subject_id")]
        public string SubjectId { set; get; }
        [BsonElement("course_subject_id")]
        public string CourseSubjectId { set; get; }
        [BsonElement("score_type_id")]
        public string ScoreTypeId { set; get; }
        [BsonElement("check_required")]
        public int CheckRequired { set; get; }
        [BsonElement("column_number")]
        public int ColumnNumber { set; get; }
        [BsonElement("average")]
        public string Average { set; get; }
        [BsonElement("multiplier")]
        public double Multiplier { set; get; }
        [BsonElement("param")]
        public string Param { set; get; }
        [BsonElement("score_subject")]
        public List<double?> ScoreSubject { set; get; }
        [BsonElement("score_created_at")]
        public DateTime ScoreCreatedAt { set; get; } = DateTime.Now;

    }
    public class ScoreReponse
    {
        public string ScoreId { set; get; } = Guid.NewGuid().ToString();
        public string StudentId { set; get; }
        public string StudentName { set; get; }
        public string ClassId { set; get; }
        public string ClassName { set; get; }
        public string SemestersId { set; get; }
        public string SemeterName { set; get; }
        public string SubjectId { set; get; }
        public string SubjectName { set; get; }
        public string CourseSubjectId { set; get; }
        public string ScoreTypeId { set; get; }
        public string ScoreTypeName { set; get; }
        public int CheckRequired { set; get; }
        public int ColumnNumber { set; get; }
        public double Multiplier { set; get; }
        public List<double?> ScoreSubject { set; get; }
        public DateTime ScoreCreatedAt { set; get; }
        public ScoreReponse() { }
        public ScoreReponse(Score score)
        {
            ScoreId = score.ScoreId;
            StudentId = score.StudentId;
            ClassId = score.ClassId;
            SemestersId = score.SemestersId;
            SubjectId = score.SubjectId;
            CourseSubjectId = score.CourseSubjectId;
            CheckRequired = score.CheckRequired;
            ColumnNumber = score.ColumnNumber;
            Multiplier = score.Multiplier;
            ScoreSubject = score.ScoreSubject;
            ScoreCreatedAt = score.ScoreCreatedAt;
            ScoreTypeId = score.ScoreTypeId;
        }

    }
}
