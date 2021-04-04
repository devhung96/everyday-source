using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Courses.Entities
{
    [Table("rc_courses")]
    public class Course
    {
        [Key]
        [Column("course_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CourseId { get; set; }
        [Column("course_name")]
        public string CourseName { get; set; }
        [Column("course_code")]
        public string CourseCode { get; set; }
        [Column("course_created")]
        public DateTime CourseCreatedAt { get; set; } = DateTime.UtcNow;
        public List<CourseInSchoolYear> CourseInSchoolYears { get; set; }
    }
}
