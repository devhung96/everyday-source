using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Entities
{
    [Table("rc_report_closing")]
    public class ReportClosing
    {
        [Key]
        [Column("report_closing_id")]
        public string ReportClosingId { set; get; } = Guid.NewGuid().ToString();

        [Column("student_id")]
        public string StudentId { set; get; }

        [Column("class_id")]
        public string ClassId { set; get; }

        [Column("subject_id")]
        public string SubjectId { set; get; }

        [Column("semester_id")]
        public string SemesterId { get; set; }

    }
}
