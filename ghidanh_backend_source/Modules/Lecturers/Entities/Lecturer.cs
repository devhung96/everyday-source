using Project.Modules.SubjectGroups.Entities;
using Project.Modules.Subjects.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Project.Modules.Students.Entities.Student;

namespace Project.Modules.Lecturers.Entities
{

    [Table("rc_lecturers")]
    public class Lecturer
    {
        [Key]
        [Column("lecturer_id")]
        public string LecturerId { get; set; } = Guid.NewGuid().ToString();
        [Column("lecturer_code")]
        public string LecturerCode { get; set; }
        [Column("lecturer_last_name")]
        public string LecturerLastName { get; set; }
        [Column("lecturer_first_name")]
        public string LecturerFistName { get; set; }    
        [Column("lecturer_tax_code")]
        public string LecturerTaxCode { get; set; }
        [Column("lecturer_birthday")]
        public DateTime LecturerBirthday { get; set; }
        [Column("lecturer_gender")]
        public GENDER LecturerGender { get; set; }
        [Column("lecturer_email")]
        public string LecturerEmail { get; set; }
        [Column("lecturer_phone")]
        public string LecturerPhone { get; set; }
        [Column("lecturer_address")]
        public string LecturerAddress { get; set; }
        [Column("lecturer_avatar")]
        public string LecturerImage { get; set; }
        [Column("lecturer_official_subject")]
        public string SubjectId { get; set; }
        [Column("account_id")]
        public string AccountId { get; set; }
        [Column("lecturer_status")]
        public STATUS LecturerStatus { get; set; } = STATUS.Active;
        [Column("lecturer_note")]
        public string LecturerNote { get; set; }
        [Column("lecturer_created")]
        public DateTime? LecturerCreated { get; set; }
        [NotMapped]
        public List<Subject> ExtraSubject { get; set; }
        [NotMapped]
        public Subject OfficialSubject { get; set; }
    }
    public class LectureSelect
    {
        public string LecturerId { get; set; } 
        public string LecturerCode { get; set; }
        public string LecturerLastName { get; set; }
        public string LecturerFistName { get; set; }
        public LectureSelect() { }
        public LectureSelect(Lecturer lec)
        {
            LecturerId = lec.LecturerId;
            LecturerCode = lec.LecturerCode;
            LecturerFistName = lec.LecturerFistName;
            LecturerLastName = lec.LecturerLastName;
        }
    }
}
