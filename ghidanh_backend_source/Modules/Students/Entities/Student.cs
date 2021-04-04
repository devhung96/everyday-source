using Project.Modules.Classes.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Students.Entities
{
    [Table("rc_students")]
    public class Student
    {
        [Key]
        [Column("student_id")]
        public string StudentId { get; set; } = Guid.NewGuid().ToString();
        [Column("student_code")]
        public string StudentCode { get; set; }
        [Column("student_last_name")]
        public string StudentLastName { get; set; }
        [Column("student_first_name")]
        public string StudentFirstName { get; set; }
        [Column("student_email")]
        public string StudentEmail { get; set; }
        [Column("student_birthdate")]
        public DateTime StudentBirthday { get; set; }
        [Column("student_gender")]
        public GENDER StudentGender { get; set; }
        [Column("student_address")]
        public string StudentAddress { get; set; }
        [Column("student_phone_number")]
        public string StudentPhone { get; set; }
        [Column("student_avatar")]
        public string StudentImage { get; set; }
        [Column("parent_name")]
        public string ParentName { get; set; }
        //[Column("mother_name")]
        //public string MotherName { get; set; }
        [Column("student_status")]
        public STATUS StudentStatus { get; set; } = STATUS.Active;
        [Column("student_created")]
        public DateTime? CreateAt { get; set; }
        [Column("student_updated")]
        public DateTime? UpdateAt { get; set; }
        [Column("account_id")]
        public string AccountId { get; set; }
        public enum GENDER
        {
            Male =0,
            Female =1,
        }
        public enum STATUS
        {
            Active =0,
            Block =1,
            Other =2
        }
        public Student() { }
        public Student(string image,string accountId, string studentCode)
        {
            this.AccountId = accountId;
            this.StudentImage = image;
            this.StudentCode = studentCode;
        }
        [NotMapped]
        public List<ClassResponse> Classes { get; set; }
     }
    public class ClassResponse
    {
        public string ClassId { get; set; }
        public string ClassCode { get; set; }
        public string ClassName { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public double ClassAmount { get; set; }
        public double SumSurcharges { get; set; }
        public EnumTuition StatusTuition { get; set; }
        public List<Surcharge> Surcharges { get; set; }

        public ClassResponse() { }
        public ClassResponse(Class @class)
        {
            ClassId = @class.ClassId;
            ClassCode = @class.ClassCode;
            ClassName = @class.ClassName;
            CourseName = @class.Course?.CourseName;
            ClassAmount = @class.ClassAmount;
            CourseId = @class.CourseID;
    }
    }
}
