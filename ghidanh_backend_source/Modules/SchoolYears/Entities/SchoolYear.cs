using Project.Modules.Semesters.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SchoolYears.Entities
{
    [Table("rc_school_years")]
    public class SchoolYear
    {
        [Key]
        [Column("school_year_id")]
        public string SchoolYearId { get; set; } = Guid.NewGuid().ToString();

        [Column("school_year_name")]
        public string SchoolYearName { get; set; }

        [Column("school_year_time_start")]
        public int TimeStart { get; set; }

        [Column("school_year_time_stop")]
        public int TimeEnd { get; set; }

        [Column("school_year_created")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //[NotMapped]
       // public List<Semester> Semesters { get; set; }
        public List<Semester> Semesters { get; set; }
    }
}
