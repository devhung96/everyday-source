using Microsoft.AspNetCore.Http;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.Courses.Entities;
using Project.Modules.SchoolYears.Entities;
using Project.Modules.Students.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Classes.Entities
{
    [Table("rc_class")]
    public class Class
    {
        [Key]
        [Column("class_id")]
        [StringLength(36)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ClassId { get; set; }
        [Column("class_code")]
        public string ClassCode { get; set; }
        [Column("class_name")]
        public string ClassName { get; set; }
        [Column("class_quantity_student")]
        public int ClassQuantityStudent { get; set; }
        [Column("course_id")]
        public string CourseID { get; set; }
        [Column("school_year_id")]
        public string SchoolYearID { get; set; }
        [Column("class_image")]
        public string ClassImage { get; set; }
        [Column("class_amount")]
        public double ClassAmount { get; set; }
        [Column("class_amount_description")]
        public string ClassAmountDescription { get; set; }
        [Column("class_description")]
        public string ClassDescription { get; set; }
        [Column("class_created")]
        public DateTime ClassCreatedAt { get; set; } = DateTime.UtcNow;
        [Column("class_admission")]
        [EnumDataType(typeof(STATUS_OPEN))]
        public STATUS_OPEN Admission { get; set; } = STATUS_OPEN.OPEN;
        [NotMapped]
        public Course Course { get; set; }
        [NotMapped]
        public SchoolYear SchoolYear { get; set; }
        [NotMapped]
        public List<Surcharge> Surcharges { get; set; }
        [NotMapped]
        public int QuantityRegisted { get; set; }
        [NotMapped]
        public EnumTuition RegistrationTuition { get; set; } = EnumTuition.NoTuition;// Trạng thái đóng HP của 1 HV trong lớp
        [NotMapped]
        public ClassSchedule  ClassSchedule { get; set; }
        public enum STATUS_OPEN
        {
            OPEN = 1,
            CLOSE = 0,
        }
    }
}
