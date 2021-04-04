using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Entities
{
    [Table("rc_score_type_subjects")]
    public class ScoreTypeSubject
    {
        [Key]
        [Column("score_type_subject_id")]
        public string ScoreTypeSubjectId { set; get; } = Guid.NewGuid().ToString();
        [Column("score_type_id")]
        public string ScoreTypeId { set; get; }
        [Column("subject_id")]
        public string SubjectId { set; get; }
        [Column("school_year_id")]
        public string SchoolYearId { set; get; }
        [Column("score_type_subject_created_at")]
        public DateTime ScoreTypeSubjectCreatedAt { set; get; } = DateTime.Now;

        public ScoreType ScoreType { get; set; }
    }
}
