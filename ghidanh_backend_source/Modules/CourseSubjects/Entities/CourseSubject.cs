using Project.Modules.ScoreTypes.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Entities
{
    [Table("rc_course_subjects")]
    public class CourseSubject
    {
        [Key]
        [Column("course_subject_id")]
        public string CourseSubjectId { set; get; } = Guid.NewGuid().ToString();
        [Column("course_id")]
        public string CourseId { set; get; }
        [Column("subject_id")]
        public string SubjectId { set; get; }
        [Column("score_type_id")]
        public string ScoreTypeId { set; get; }
        [Column("column_number")]
        public int ColumnNumber { set; get; }
        [Column("check_required_column_number")]
        public int CheckRequired { set; get; }
        [Column("course_subject_created_at")]
        public DateTime CourseSubjectCreatedAt { set; get; } = DateTime.UtcNow;

        public ScoreType ScoreType { get; set; }
    }
    public class CourseSubjectResponse
    {
        public string CourseSubjectId { set; get; }
        public string CourseId { set; get; }
        public string CourseName { set; get; }
        public string SubjectId { set; get; }
        public string SubjectName { set; get; }
        public string ScoreTypeId { set; get; }
        public string ScoreTypeName { get; set; }
        public double? Multiplier { get; set; }
        public int ColumnNumber { set; get; }
        public int CheckRequired { set; get; }
        public DateTime CourseSubjectCreatedAt { set; get; }
        public CourseSubjectResponse() { }
        public CourseSubjectResponse(CourseSubject courseSubject) 
        {
            CourseSubjectId = courseSubject.CourseSubjectId;
            CourseId = courseSubject.CourseId;
            SubjectId = courseSubject.SubjectId;
            ColumnNumber = courseSubject.ColumnNumber;
            ScoreTypeId = courseSubject.ScoreTypeId;
            CourseSubjectCreatedAt = courseSubject.CourseSubjectCreatedAt;
            CheckRequired = courseSubject.CheckRequired;
        }
    }
}
