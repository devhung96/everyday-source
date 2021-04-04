using Microsoft.EntityFrameworkCore.Query.Internal;
using Project.Modules.Classes.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Students.Entities
{
    [Table("rc_registration_studies")]
    public class RegistrationStudy
    {
        [Key]
        [Column("registration_study_id")]
        public string RegistrationStudyId { get; set; } = Guid.NewGuid().ToString();
        [Column("student_id")]
        public string StudentId { get; set; }
        [Column("class_id")]
        public string ClassId { get; set; }
        [Column("registration_tuition")]
        public EnumTuition RegistrationTuition { get; set; } = EnumTuition.NoTuition;
        [Column("registration_tuition_date")]
        public DateTime? RegistrationTuitionDate { get; set; }
        [Column("registration_created")]
        public DateTime? CreatedAt { get; set; }
        public Student Student { get; set; }      
        public Class Class { get; set; }
        [NotMapped]
        public string Token { get; set; }
        public RegistrationStudy(string studentId, string classId)
        {
            this.ClassId = classId;
            this.StudentId = studentId;
        }
        
    }
    public class StudyResponse
    {
        public string RegistrationStudyId { get; set; }
        public string StudentId { get; set; }
        public string StudentCode { get; set; }
        public string StudentLastName { get; set; }
        public string StudentFirstName  { get; set; }
        public string StudentEmail { get; set; }
        public DateTime StudentBirthday { get; set; }
        public string StudentAddress { get; set; }
        public string StudentPhone { get; set; }
        public string StudentImage { get; set; }
        public string ParentName { get; set; }
        public string ClassId { get; set; }
        public string ClassCode { get; set; }
        public string ClassName { get; set; }
        public string CourseName { get; set; }
        public EnumTuition RegistrationTuition { get; set; }
        public DateTime? RegistrationTuitionDate { get; set; }

        public StudyResponse() { }
        public StudyResponse(RegistrationStudy Reg)
        {
            RegistrationStudyId = Reg.RegistrationStudyId;
            StudentId = Reg.Student.StudentId;
            StudentCode = Reg.Student.StudentCode;
            StudentLastName = Reg.Student.StudentLastName;
            StudentFirstName = Reg.Student.StudentFirstName;
            StudentEmail = Reg.Student.StudentEmail;
            StudentBirthday = Reg.Student.StudentBirthday;
            StudentAddress = Reg.Student.StudentAddress;
            StudentPhone = Reg.Student.StudentPhone;
            StudentImage = Reg.Student.StudentImage;
            ParentName = Reg.Student.ParentName;
            ClassId = Reg.Class.ClassId;
            ClassCode = Reg.Class.ClassCode;
            ClassName = Reg.Class.ClassName;
            CourseName = Reg.Class.Course == null ? null : Reg.Class.Course.CourseName;
            RegistrationTuition = Reg.RegistrationTuition;
            RegistrationTuitionDate = Reg.RegistrationTuitionDate;
        }
    }
    public enum EnumTuition
    {
        CollectedTuition = 1,
        NoTuition = 0
    }
}
