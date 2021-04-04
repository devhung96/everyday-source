using Project.Modules.SchoolYears.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Semesters.Entities
{
    [Table("rc_semesters")]
    public class Semester
    {
        [Key]
        [Column("semester_id")]
        public string SemesterId { get; set; } = Guid.NewGuid().ToString();

        [Column("school_year_id")]
        public string SchoolYearId { get; set; }

        [Column("semester_name")]
        public string SemesterName { get; set; }

        [Column("semester_timestart")]
        public DateTime TimeStart { get; set; }

        [Column("semester_timeend")]
        public DateTime TimeEnd { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        //[NotMapped]
        //public SchoolYears SchoolYears { get; set; }
    }
}
