using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Courses.Entities
{
    [Table("rc_course_school_years")]
    public class CourseInSchoolYear
    {
        [Key]
        [Column("course_school_year_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CourseSchoolYearId { get; set; }
        [Column("course_id")]
        public string CourseId { get; set; }
        [Column("school_year_id")]
        public string SchoolYearId { get; set; }
        [Column("course_school_year_create_at")]
        public DateTime CourseSchoolYearCreatAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public Course Course { get; set; }
    }
}
