using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.Databases;
using Project.Modules.Classes.Entities;
using Project.Modules.Courses.Entities;
using Project.Modules.CourseSubjects.Entities;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.SubjectGroups.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Project.Modules.Subjects.Entities
{
    [Table("rc_subjects")]
    public class Subject
    {
        [Key]
        [Column("subject_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SubjectId { get; set; }
        [Column("subject_code")]
        public string SubjectCode { get; set; }
        [Column("subject_name")]
        public string SubjectName { get; set; }
        [Column("course_id")]
        public string CourseId { get; set; }
        [Column("subject_group_id")]
        public string SubjectGroupId { get; set; }
        [Column("subject_created")]
        public DateTime SubjectCreatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public string SubjectGroupName { get; set; }
        [NotMapped]
        public string CourseName { get; set; }

        [NotMapped]
        public List<Class> Classes
        {
            get 
            {
                if (CourseId != null)
                {
                    var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
                    optionsBuilder.UseMySql(ExtensionStatic.Configuration["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
                    var mariaDBContext = new MariaDBContext(optionsBuilder.Options);
                    return mariaDBContext.Classes.Where(x => x.CourseID.Equals(CourseId) && x.Admission == Class.STATUS_OPEN.OPEN).ToList();
                }
                return new List<Class>();
            }
        }
    }
}
