using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Entities
{
    [Table("rc_lecturer_subject")]
    public class LecturerSubject
    {
        [Key]
        [Column("lecturer_subject_id")]
        public string LecturerSubjectId { get; set; } = Guid.NewGuid().ToString();
        [Column("lecturer_id")]
        public string LecturerId { get; set; }
        [Column("subject_id")]
        public string SubjectId { get; set; }
        [Column("lecturer_subject_created")]
        public DateTime? CreatedAt { get; set; }
    }
}
